using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Ns.BpmOnline.Worker
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main(string[] args)
        {
          

            if (Environment.UserInteractive)
            {
                WorkerService service1 = new WorkerService();
                service1.TestStartupAndStop(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new WorkerService()
                };
                ServiceBase.Run(ServicesToRun);

            }
        }
    }
}
