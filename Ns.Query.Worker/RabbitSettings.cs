using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Ns.BpmOnline.Worker
{
    public interface IRabbitSettings
    {
        string ExchangeName { get; }
        string QueueName { get; }
        string RoutingKey { get; }
        //string RequestQueueNameString { get; }
        //string AnswerQueueNameString { get; }

    }

    abstract public class RabbitSettings
    {

        public RabbitSettings()
        {  
        }
    }

    public class QueryExecutorRabbitSettings : RabbitSettings, IRabbitSettings
    {
        public QueryExecutorRabbitSettings(string exchangeName = "") {
            //Logger.Log(String.Format("{0} | {1} | {2} | {3} | {4}", ExchangeName, RequestQueueNameString, AnswerQueueNameString, RoutingKey, AnswerQueueName));
           //Logger.Log(String.Format("{0}|{1}|{2}|{3}", ExchangeName, QueueName, RoutingKey, AnswerQueueName));

        }
        public string ExchangeName => ConfigurationManager.AppSettings["exchangeName"];
        public string QueueName => String.Format("{0}_{1}", ExchangeName, "SQL_TO_EXECUTE");
        public string RoutingKey => String.Format("{0}_{1}", ExchangeName, "SQL_TO_EXECUTE");
        public string AnswerQueueName => String.Format("{0}_{1}", QueueName, "ANSWER");
        public string AnswerRoutingKey => String.Format("{0}_{1}", QueueName, "ANSWER");
        /*        public string ExchangeName => ConfigurationManager.AppSettings["exchangeName"];
                public string RequestQueueNameString => ConfigurationManager.AppSettings["requestQueueName"];
                public string AnswerQueueNameString => ConfigurationManager.AppSettings["answerQueueName"];

                public string QueueName => String.Format("{0}_{1}", ExchangeName, RequestQueueNameString);
                public string RoutingKey => String.Format("{0}_{1}", ExchangeName, RequestQueueNameString);
                public string AnswerQueueName => String.Format("{0}_{1}", ExchangeName, AnswerQueueNameString);
                public string AnswerRoutingKey => String.Format("{0}_{1}", ExchangeName, AnswerQueueNameString);*/


    }

    public class NsReceivedQuery
    {
        public string ProcessId { get; set; }
        public string ProcessElementId { get; set; }
        public string Query { get; set; }
        public string ID { get; set; }
        public bool IsNeedResult { get; set; }
        public bool UseQueryInResult { get; set; }
        public string ResultTable { get; set; }
        public string ResultColumn { get; set; }
        public string QueryResultType { get; set; }
        public NsSqlRequestParameters RequestParameters { get; set; }
    }

    public class NsQueryToSendAnswer
    {
        public string ProcessId { get; set; }
        public string ProcessElementId { get; set; }
        public string Result { get; set; }
        public string ID { get; set; }
        public string Status { get; set; }
        public int AffectedRows { get; set; }
        public string ResultTable { get; set; }
        public int ExecutionTime { get; set; }
        public bool IsNeedResult { get; set; }
        public string QueryResultType { get; set; }
        public string QueryResult { get; set; }
        public NsSqlRequestParameters RequestParameters { get; set; }
    }

    public class NsSqlRequestParameters
    {
        public NsSqlRequestType Type { get; set; } = NsSqlRequestType.ContinueProcess;
        public string Code { get; set; }
    }

    public enum NsSqlRequestType
    {
        ContinueProcess,
        SaveParameters
    }

}