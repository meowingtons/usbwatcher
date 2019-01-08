using System.Diagnostics;
using System.ServiceProcess;
using UsbWatcher.EventLog;
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
            var entry = new Event
            {
                Action = "UsbWatcher service stopped",
                EventId = EventIds.ServiceStopped,
                EventType = EventLogEntryType.SuccessAudit,
                EventBody = "The UsbWatcher service was stopped gracefully."
            };

            Logger.WriteLog(entry);
        }
    }
}
