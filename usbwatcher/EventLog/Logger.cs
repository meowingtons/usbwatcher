using System;
using USBLogging;

namespace UsbWatcher.EventLog
{
    public static class Logger
    {
        public const string EventSource = "UsbWatcher";
        public const string EventLog = "Application";
        public const string EventMachine = ".";

        private static System.Diagnostics.EventLog _log;
        
        public static void WriteLog(Event evt)
        {
            _log = new System.Diagnostics.EventLog(EventLog, EventMachine, EventSource);

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
}