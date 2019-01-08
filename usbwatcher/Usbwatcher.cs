using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using USBLogging;
using UsbWatcher.Watchers;

namespace UsbWatcher
{
    public partial class UsbWatcherService : ServiceBase
    {
        private static DriveWatcher _watcher;

        public UsbWatcherService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Utils.ConfigureEventLog();
            DriveWatcher.FindExistingDrives();

            _watcher = new DriveWatcher();
            _watcher.StartWatching();
        }

        protected override void OnStop()
        {
        }
    }
}
