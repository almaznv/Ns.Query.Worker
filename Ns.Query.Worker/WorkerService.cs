using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using RabbitMQ.Client;
using Ns.BpmOnline.Worker.Executors;
using Sider;

namespace Ns.BpmOnline.Worker
{
    

    public partial class WorkerService : ServiceBase
    {

        private List<IRabbitConsumer> _consumers;

        public WorkerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //var client = new RedisClient("localhost");
            //client.Select(15);
           // client.HSet("sdfsdf", "name", "andrew");

            RegisterConsumers();

        }


        private void RegisterConsumers()
        {
            IConnection connection = RabbitConnector.GetConnection();
            _consumers = new List<IRabbitConsumer>()
            {
                new CommandConsumer(connection, new QueryExecutor(), new QueryExecutorRabbitSettings()),
            };
        }

        protected override void OnStop()
        {
            IConnection connection = RabbitConnector.GetConnection();
            foreach (var consumer in _consumers)
            {
                consumer.Close();
            }
            connection.Close();
        }

        internal void TestStartupAndStop(string[] args)  
        {  
            this.OnStart(args);  
            //Console.ReadLine();  
            this.OnStop();  
        }

    }
}
