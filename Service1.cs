using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WindowsServiceTest1.Service;

namespace WindowsServiceMoveFiles
{
    public partial class Service1 : ServiceBase
    {
        private Watcher fWatcher = Watcher.Instance;
       
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            fWatcher.WriteToFile("Service is started at " + DateTime.Now);
            fWatcher.LogEventStart();
            fWatcher.WriteToFile("After Service is started at " + DateTime.Now);
            fWatcher.GetWatcherForDirectory(@"C:\Users\ASUS\MultiSys\source");
        }

        protected override void OnStop()
        {
            fWatcher.WriteToFile("Service is stopped at " + DateTime.Now);
            fWatcher.LogEventSTop();
        }
    }
}
