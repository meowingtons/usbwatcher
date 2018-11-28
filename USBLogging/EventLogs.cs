using System.Diagnostics;

namespace USBLogging
{
    public class Logger
    {
        public const string EventSource = "UsbWatcher";
        public const string EventLog = "Application";
        public const string EventMachine = ".";

        private static EventLog _log;

        public Logger()
        {
            _log = new EventLog(EventLog, EventMachine, EventSource);
        }
        
        public void WriteLog()
        {
            _log.WriteEntry("message", EventLogEntryType.SuccessAudit, (int)EventIds.DriveInserted);
        }

        public enum EventIds
        {
            DriveInserted = 69001,
            DriveRemoved = 69002,
            DriveDiscoveredOnStart = 69003,
            WatcherStarted = 69200,
            WatcherStopped = 69201,
            FileWritten = 69101,
            FileRenamed = 69102,
            FileDeleted = 69103,
            FileChanged = 69104,
            FileError = 69691,
            OtherError = 69690
        }
    }
}