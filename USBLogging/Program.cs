﻿using System;
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
            
            FindExistingDrives();
            
            _watcher = new DriveWatcher();
            _watcher.StartWatching();
            
            Thread.Sleep(Timeout.Infinite);
            
            Console.WriteLine("Press \'q\' to quit the sample.");
            while(Console.Read()!='q');
        }

        private static void FindExistingDrives()
        {
            var driveList = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Removable);

            foreach (var drive in driveList)
            {
                Console.WriteLine("Found Existing Drive: " + drive.Name);
                Console.WriteLine("Starting Watcher on: " + drive.Name);
                FileWatcher.Run(drive.Name);
            }
        }

        private static void ConfigureEventLog()
        {
            if (!EventLog.SourceExists(Logger.EventSource, Logger.EventMachine))
            {
                Console.WriteLine("Event log source doesn't exist. Creating now.");
                EventLog.CreateEventSource(new EventSourceCreationData(Logger.EventSource, Logger.EventLog));
            }
        }
    }
}