using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using USBLogging;
using System.Diagnostics;
using System.Threading;

namespace MonitorDrives
{
    class Program
    {
        private static DriveWatcher _watcher;
        
        static void Main(string[] args)
        {          
            ConfigureEventLog();
                        
            DriveWatcher.FindExistingDrives();
            
            _watcher = new DriveWatcher();
            _watcher.StartWatching();
            
            Thread.Sleep(Timeout.Infinite);
        }


        private static void ConfigureEventLog()
        {
            if (EventLog.SourceExists(Logger.EventSource, Logger.EventMachine)) return;
            Console.WriteLine("Event log source doesn't exist. Creating now.");
            EventLog.CreateEventSource(new EventSourceCreationData(Logger.EventSource, Logger.EventLog));
        }
    }
}