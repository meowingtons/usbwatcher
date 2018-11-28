using System;
using System.ComponentModel;
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

            Console.WriteLine("{0}: {1} {2}", DateTime.Now, drivePath, eventName);

            switch (eventType)
            {
                case EventType.Inserted:
                    FileWatcher.Run(drivePath);

                    var driveProps = DriveProperties.GetDeviceProperties(diskLetter);

                    foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(driveProps))
                    {
                        var name=descriptor.Name;
                        var value=descriptor.GetValue(driveProps);
                        Console.WriteLine("{0}={1}", name, value);
                    }
                    break;
                case EventType.Removed:
                    FileWatcher.Stop(drivePath);
                    break;
            }
        }
    }
}