using System;
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
            try
            {
                Utils.ConfigureEventLog();
                DriveWatcher.FindExistingDrives();

                _watcher = new DriveWatcher();
                _watcher.StartWatching();
            }
            catch (Exception e)
            {
                var entry = new Event
                {
                    Action = "UsbWatcher service encountered an error. Attempting to restart.",
                    EventId = EventIds.OtherError,
                    EventType = EventLogEntryType.Error,
                    EventBody = e.Message
                };

                Logger.WriteLog(entry);

                try
                {
                    Utils.ConfigureEventLog();
                    DriveWatcher.FindExistingDrives();

                    _watcher = new DriveWatcher();
                    _watcher.StartWatching();
                }
                catch
                {
                    var secondEntry = new Event
                    {
                        Action = "UsbWatcher service had an error restarting. Not attempting to restart.",
                        EventId = EventIds.OtherError,
                        EventType = EventLogEntryType.Error,
                        EventBody = e.Message
                    };

                    Logger.WriteLog(secondEntry);
                }
            }
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
