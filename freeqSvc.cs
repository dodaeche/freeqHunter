using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace freeqHunter
{
    public partial class freeqSvc : ServiceBase
    {

        Thread sysmonThread;
        Thread uploadThread;
       

        public freeqSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ThreadStart eventThreadDelegate = new ThreadStart(SysmonWatcher.Run);
            sysmonThread = new Thread(eventThreadDelegate);
            sysmonThread.Start();

            ThreadStart uploadThreadDelegate = new ThreadStart(Uploader.UploadFiles);
            uploadThread = new Thread(uploadThreadDelegate);
            uploadThread.Start();

        }

        protected override void OnStop()
        {
        }
    }
}
