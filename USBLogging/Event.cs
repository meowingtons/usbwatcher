using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Xml.Serialization;

namespace USBLogging
{
    public class Event
    {
        private DriveProperties _driveProperties;

        private static string Serialize<T>(T dataToSerialize)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, dataToSerialize);
            return stringwriter.ToString();
        }

        public DriveProperties DriveProperties
        {
            get => DriveProperties.GetDeviceProperties(DriveLetter);
        }
        public string FullFilePath { get; set; }
        public DateTime Time => DateTime.Now;

        public EventLogEntryType EventType { get; set; }
        public EventIds EventId { get; set; }
        public string EventBody { get; set; }
        public string DriveLetter { get; set; }
        public string Action { get; set; }
    }
}