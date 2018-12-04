using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace USBLogging
{
    public enum EventType
    {
        Inserted = 2,
        Removed = 3
    }

    public class DriveWatcher
    {
        private readonly ManagementEventWatcher _watcher;

        public DriveWatcher()
        {
            var query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 or EventType = 3");
            _watcher = new ManagementEventWatcher(query);
        }
        
        public static void FindExistingDrives()
        {
            var driveList = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Removable);

            foreach (var drive in driveList)
            {
                var entry = new Event
                {
                    DriveLetter = drive.Name.Replace("\\", ""),
                    Action = "Drive Discovered.",
                    EventId = EventIds.DriveDiscoveredOnStart,
                    EventType = EventLogEntryType.SuccessAudit,
                    EventBody = "On start, the following drive was discovered: " + drive.Name
                };

                Logger.WriteLog(entry);
                ProcessEventType(drive.Name.Replace("\\", ""), drive.Name, EventType.Inserted);
            }
        }

        private static void ProcessEventType(string diskLetter, string drivePath, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Inserted:
                    FileWatcher.Run(drivePath);

                    var driveProps = DriveProperties.GetDeviceProperties(diskLetter);
                    var props = "";

                    props += "Action: Drive Inserted" + Environment.NewLine;

                    foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(driveProps))
                    {
                        var name=descriptor.Name;
                        var value=descriptor.GetValue(driveProps);
                        props += name + ": " + value + System.Environment.NewLine;
                    }
                    
                    var entry = new Event();
                    entry.DriveLetter = diskLetter;
                    entry.Action = "Drive inserted.";
                    entry.EventId = EventIds.DriveInserted;
                    entry.EventType = EventLogEntryType.SuccessAudit;
                    entry.EventBody = props;
                    
                    Logger.WriteLog(entry);
                    break;
                case EventType.Removed:
                    FileWatcher.Stop(drivePath);
                    break;
            }
        }


        public void StartWatching()
        {
            _watcher.EventArrived += new EventArrivedEventHandler(ProcessEvent);
            _watcher.Start();
        }

        private static void ProcessEvent(object s, EventArrivedEventArgs e)
        {
            var diskLetter = e.NewEvent.Properties["DriveName"].Value.ToString();
            var drivePath = diskLetter + "\\";
            var eventType = (EventType)(Convert.ToInt16(e.NewEvent.Properties["EventType"].Value));

            var eventName = Enum.GetName(typeof(EventType), eventType);

            ProcessEventType(diskLetter, drivePath, eventType);
        }
    }
}