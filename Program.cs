using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace freeqHunter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        internal static WorkstationRegistration Endpoint = new WorkstationRegistration();
        internal static string API_VERSION = "api/1.0";
        internal static string DEFAULT_URL = "";

        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new freeqSvc()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
