using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Sider;

namespace Ns.BpmOnline.Worker.Executors
{

    public class QueryExecutor : Executor, IExecutor
    {

        private string sqlConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["dbConnectionString"];
            }
        }

        public QueryExecutor() : base() { }

        public void Execute(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            NsReceivedQuery receivedQuery = JsonConvert.DeserializeObject<NsReceivedQuery>(json);
            Execute(receivedQuery);
        }

        public void Execute(NsReceivedQuery receivedQuery)
        {

            Logger.Log(String.Format("Incoming query params. prId:{0}, prElId:{1}, id:{2}, isNeedResult:{3} , query: {4}",
                        receivedQuery.ProcessId, receivedQuery.ProcessElementId, receivedQuery.ID, receivedQuery.IsNeedResult, receivedQuery.Query));

            try
            {
                Task<int> task = RunExecuteQueryAsync(receivedQuery);
                int result = task.Result;
            }
            catch (Exception e)
            {
                Logger.Log(String.Format("Error query: {0}", e.Message));
                SendAnswerError(receivedQuery, e.Message);
            }
        }

        private void SendAnswerError(NsReceivedQuery receivedQuery, string answerResult)
        {
            var answer = new NsQueryToSendAnswer()
            {
                ProcessId = receivedQuery.ProcessId,
                ProcessElementId = receivedQuery.ProcessElementId,
                ID = receivedQuery.ID,
                Result = answerResult,
                Status = "ERROR"
            };
            var message = JsonConvert.SerializeObject(answer);
            SendAnswer(message);
        }

        private void SendAnswerSuccess(NsReceivedQuery receivedQuery, int affectedRows, string resultTable, int executionTime, string answerResult, string queryResult)
        {
            var answer = new NsQueryToSendAnswer()
            {
                ProcessId = receivedQuery.ProcessId,
                ProcessElementId = receivedQuery.ProcessElementId,
                ID = receivedQuery.ID,
                Result = answerResult,
                Status = "OK",
                AffectedRows = affectedRows,
                ResultTable = resultTable,
                ExecutionTime = executionTime,
                IsNeedResult = receivedQuery.IsNeedResult,
                QueryResultType = receivedQuery.QueryResultType,
                QueryResult = queryResult,
                RequestParameters = receivedQuery.RequestParameters

            };
            var message = JsonConvert.SerializeObject(answer);
            SendAnswer(message);
        }

        private void SendAnswer(string message)
        {
            var connection = RabbitConnector.GetConnection();
            var settings = new QueryExecutorRabbitSettings();
            RabbitPublisher.Publish(connection, settings.ExchangeName, settings.AnswerQueueName, settings.AnswerRoutingKey, message);
        }

        private Task<int> RunExecuteQueryAsync(NsReceivedQuery receivedQuery)
        {
            var tcs = new TaskCompletionSource<int>();

            Task.Factory.StartNew(() =>
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                ExecuteQuery(receivedQuery,
                (int affected, string errText, string queryResult) =>
                {
                    watch.Stop();
                    if (affected >= 0)
                    {
                        int executionTime = (int) watch.ElapsedMilliseconds / 1000;
                        string affectedRows = String.Format("Success affected rows: {0}", affected.ToString());

                        SendAnswerSuccess(receivedQuery, affected, receivedQuery.ResultTable, executionTime, affectedRows, queryResult);

                        Logger.Log(String.Format("Sent answer to {0} query. Success affected rows: {1}", receivedQuery.ID, affected.ToString()));

                        tcs.SetResult(1);
                    }
                    else if (affected < 0)
                    {
                        SendAnswerError(receivedQuery, errText);
                        tcs.SetResult(-1);
                    }


                });
            });
            return tcs.Task;
        }

        private void ExecuteQuery(NsReceivedQuery receivedQuery, Action<int, string, string> Callback)
        {
            int count;
            string errText;

            try
            {
                
                string queryResult = String.Empty;

                if (receivedQuery.IsNeedResult && receivedQuery.QueryResultType == "redis")
                {
                    (count, errText) = ExecuteTspWithoutResult(receivedQuery.Query, receivedQuery.ID);
                    InsertResultToRedis(receivedQuery);
                }
                else if (receivedQuery.IsNeedResult && receivedQuery.QueryResultType == "multiple")
                {
                    (count, errText, queryResult) = GetMultipleResult(receivedQuery);
                }
                else if (receivedQuery.IsNeedResult && receivedQuery.QueryResultType == "one")
                {
                    (count, errText, queryResult) = GetMultipleResult(receivedQuery);
                }
                else
                {
                    (count, errText) = ExecuteTspWithoutResult(receivedQuery.Query, receivedQuery.ID);
                }


                Callback(count, errText, queryResult);

            }
            catch (Exception e)
            {
                Callback(-1, e.Message, String.Empty);
            }
        }

        private (int, string) ExecuteTspWithoutResult(string sqlQuery, string Id)
        {
            int returnValue = 0;
            string errText = "s";

            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("tsp_NsPerformQueryWithoutResult", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@sqlQuery", sqlQuery));
                cmd.Parameters.Add(new SqlParameter("@queryId", Id));

                SqlParameter countParam = new SqlParameter("@affected", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(countParam);

                SqlParameter errTextParam = new SqlParameter("@err", SqlDbType.NVarChar) { Direction = ParameterDirection.Output, Size = 4000 };
                cmd.Parameters.Add(errTextParam);

                connection.Open();

                cmd.CommandTimeout = 300;

                cmd.ExecuteNonQuery();

                returnValue = countParam.Value as int? ?? default(int);

                errText = (string)errTextParam.Value;

                connection.Close();
            }

            return (returnValue, errText);
        }

        private (int, string, string) GetOneResult(NsReceivedQuery receivedQuery)
        {
            return (0, String.Empty, String.Empty);
        }

        private (int, string, string) GetMultipleResult(NsReceivedQuery receivedQuery)
        {
            var commandText = receivedQuery.Query;
            int count = 0;

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            SqlConnection conn = new SqlConnection(sqlConnectionString);
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    var cols = new List<string>();
                    for (var j = 0; j < reader.FieldCount; j++)
                    {
                        cols.Add(reader.GetName(j));
                    }

                    while (reader.Read())
                    {
                        result.Add(SerializeRow(cols, reader));
                        count++;
                    }
                   
                }

            }

            return (count, String.Empty, JsonConvert.SerializeObject(result));
        }

        private void InsertResultToRedis(NsReceivedQuery receivedQuery)
        {
            var commandText = (!receivedQuery.UseQueryInResult)
                ? $"SELECT {receivedQuery.ResultColumn} FROM {receivedQuery.ResultTable} WHERE QueryId = '{receivedQuery.ID}'"
                : receivedQuery.Query;

            SqlConnection conn = new SqlConnection(sqlConnectionString);
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {

                    var cols = new List<string>();
                    for (var j = 0; j < reader.FieldCount; j++)
                    {
                        cols.Add(reader.GetName(j));
                    }
                    var client = NsRedisHelper.getRedisClient();

                    var key = $"nsResult_{receivedQuery.ID}";
                    client.Multi();

                    while (reader.Read())
                    {
                        string jsonStr = JsonConvert.SerializeObject(SerializeRow(cols, reader), Formatting.Indented);
                        client.LPush(key, jsonStr);
                    }
                    client.Exec();
                    client.Dispose();
                }

            }

        }

        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols, SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
            {
                if (!result.ContainsKey(col))
                {
                    result.Add(col, reader[col]);
                }
            }

            return result;
        }
    }
}
