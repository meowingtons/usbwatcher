using System;
using System.Management;
using USBLogging;

namespace MonitorDrives
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

        public void StartWatching()
        {
            _watcher.EventArrived += new EventArrivedEventHandler(ProcessEvent);
            _watcher.Start();
        }

        private static void ProcessEvent(object s, EventArrivedEventArgs e)
        {
            string drivePath = e.NewEvent.Properties["DriveName"].Value.ToString() + "\\";
            EventType eventType = (EventType)(Convert.ToInt16(e.NewEvent.Properties["EventType"].Value));

            string eventName = Enum.GetName(typeof(EventType), eventType);

            Console.WriteLine("{0}: {1} {2}", DateTime.Now, drivePath, eventName);

            if (eventType == EventType.Inserted)
            {
                FileWatcher.Run(drivePath);
            }

            if (eventType == EventType.Removed)
            {
                FileWatcher.Stop(drivePath);
            }
        }
    }
}