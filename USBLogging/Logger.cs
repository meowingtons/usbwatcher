using System;
using System.Diagnostics;

namespace USBLogging
{
    public static class Logger
    {
        public const string EventSource = "UsbWatcher";
        public const string EventLog = "Application";
        public const string EventMachine = ".";

        private static EventLog _log;
        
        public static void WriteLog(Event evt)
        {
            _log = new EventLog(EventLog, EventMachine, EventSource);

            try
            {
                _log.WriteEntry(evt.EventBody, evt.EventType, (int) evt.EventId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
    
    public enum EventIds
    {
        DriveInserted = 59001,
        DriveRemoved = 59002,
        DriveDiscoveredOnStart = 59003,
        WatcherStarted = 59200,
        WatcherStopped = 59201,
        FileWritten = 59101,
        FileRenamed = 59102,
        FileDeleted = 59103,
        FileChanged = 59104,
        FileError = 59591,
        OtherError = 59590
    }
}