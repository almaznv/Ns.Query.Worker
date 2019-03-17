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
    }

    abstract public class RabbitSettings
    {

        public RabbitSettings()
        {  
        }
    }

    public class QueryExecutorRabbitSettings : RabbitSettings, IRabbitSettings
    {
        public QueryExecutorRabbitSettings(string exchangeName = "") { }
        public string ExchangeName => ConfigurationManager.AppSettings["exchangeName"];
        public string QueueName => String.Format("{0}_{1}", ExchangeName, "SQL_TO_EXECUTE");
        public string RoutingKey => String.Format("{0}_{1}", ExchangeName, "SQL_TO_EXECUTE");
        public string AnswerQueueName => String.Format("{0}_{1}", QueueName, "ANSWER");
        public string AnswerRoutingKey => String.Format("{0}_{1}", QueueName, "ANSWER");
    }

    public class NsReceivedQuery
    {
        public string ProcessId { get; set; }
        public string ProcessElementId { get; set; }
        public string Query { get; set; }
        public string ID { get; set; }
        public bool IsNeedResult { get; set; }
        public string ResultTable { get; set; }
        public string ResultColumn { get; set; }
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
    }

}