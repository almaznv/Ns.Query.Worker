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
            } catch (Exception e)
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

        private void SendAnswerSuccess(NsReceivedQuery receivedQuery, int affectedRows, string resultTable, int executionTime, string answerResult)
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
                ExecutionTime = executionTime

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

                ExecuteQuery(receivedQuery.Query, receivedQuery.ID, receivedQuery.ResultTable, receivedQuery.ResultColumn, 
                (int affected) => {

                    watch.Stop();

                    SendAnswerSuccess(receivedQuery, affected, receivedQuery.ResultTable, (int)watch.ElapsedMilliseconds/1000, String.Format("Success affected rows: {0}", affected.ToString()));

                    Logger.Log(String.Format("Sent answer to {0} query. Success affected rows: {1}", receivedQuery.ID, affected.ToString()));
                    tcs.SetResult(1);

                }, (Exception e) =>
                {
                    tcs.SetException(e);
                });
            });
            return tcs.Task;
        }

        private void ExecuteQuery(string sqlQuery, string Id, string ResultTable, string ResultColumn, Action<int> SuccessCallback, Action<Exception> ErrorCallback)
        {
            try
            {
                    var count = ExecuteTspWithoutResult(sqlQuery, Id);

                    SuccessCallback(count);
                
            } catch (Exception e) {
                ErrorCallback(e);
            }
        }

        private int ExecuteTspWithoutResult(string sqlQuery, string Id)
        {
            int returnValue = 0;

            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("tsp_NsPerformQueryWithoutResult", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;                cmd.Parameters.Add(new SqlParameter("@sqlQuery", sqlQuery));
                cmd.Parameters.Add(new SqlParameter("@queryId", Id));

                SqlParameter countParam = new SqlParameter("@affected", SqlDbType.Int) { Direction = ParameterDirection.Output };                cmd.Parameters.Add(countParam);


                connection.Open();

                cmd.ExecuteNonQuery();

                returnValue = countParam.Value as int? ?? default(int);

                connection.Close();
            }

            return returnValue;
        }

        private int ExecuteTspWithResult(string sqlQuery, string Id, string ResultTable, string ResultColumn)
        {
            int i = 0;

            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("tsp_NsPerformQueryWithResult", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;                cmd.Parameters.Add(new SqlParameter("@sqlQuery", sqlQuery));
                cmd.Parameters.Add(new SqlParameter("@queryId", Id));
                cmd.Parameters.Add(new SqlParameter("@resultTable", ResultTable));
                cmd.Parameters.Add(new SqlParameter("@resultColumn", ResultColumn));


                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    
                    // Read advances to the next row.
                    while (reader.Read()) i++;

                }

                connection.Close();
            }

            return i;
        }
    }
}
