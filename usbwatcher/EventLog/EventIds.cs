namespace UsbWatcher.EventLog
{
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
        OtherError = 59590,
        ServiceStopped = 59600,
        ServiceStarted = 59601
    }
}