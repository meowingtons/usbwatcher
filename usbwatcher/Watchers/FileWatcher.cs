using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using USBLogging;
using UsbWatcher.EventLog;

namespace UsbWatcher.Watchers
{
    public static class FileWatcher
    {
        private static FileSystemWatcher _watcher;
        
        [PermissionSet(SecurityAction.Demand, Name="FullTrust")]
        public static void Run(string driveLetter)
        {
            _watcher = new FileSystemWatcher
            {
                Path = driveLetter,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                                        | NotifyFilters.FileName
                                                        | NotifyFilters.DirectoryName
                                                        | NotifyFilters.Size
            };

            _watcher.Changed += new FileSystemEventHandler(OnChanged);
            _watcher.Created += new FileSystemEventHandler(OnCreated);
            _watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed);
            _watcher.Error += new ErrorEventHandler(OnError);

            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;

            var entry = new Event
            {
                DriveLetter = driveLetter,
                Action = "File watcher started.",
                EventId = EventIds.WatcherStarted,
                EventType = EventLogEntryType.SuccessAudit,
                EventBody = "File watcher was successfully started on drive: " + driveLetter
            };

            Logger.WriteLog(entry);
        }

        public static void Stop(string driveLetter)
        {
            _watcher.Dispose();

            var entry = new Event
            {
                DriveLetter = driveLetter,
                Action = "File watcher started.",
                EventId = EventIds.WatcherStarted,
                EventType = EventLogEntryType.SuccessAudit,
                EventBody = "File watcher was successfully stopped on drive: " + driveLetter
            };

            Logger.WriteLog(entry);
        } 
            
        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            LogFileAction(e.FullPath, e.ChangeType.ToString(), e.FullPath, EventIds.FileChanged);
        }
        
        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            LogFileAction(e.FullPath, e.ChangeType.ToString(), e.FullPath, EventIds.FileWritten);
        }
        
        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            LogFileAction(e.FullPath, e.ChangeType.ToString(), e.FullPath, EventIds.FileDeleted);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            LogFileAction(e.FullPath, e.ChangeType.ToString(), e.FullPath, EventIds.FileRenamed);
        }

        private static void OnError(object source, ErrorEventArgs e)
        {
            LogFileAction("Drive Error", "Error", "", EventIds.FileError);
        }

        public static void LogFileAction(string diskLetter, string action, string fullPath, EventIds id)
        {
            var body = "Action: " + action + Environment.NewLine;

            if (id != EventIds.FileError || id != EventIds.OtherError)
            {
                body += "FullPath: " + fullPath + Environment.NewLine;

                if (id != EventIds.FileDeleted)
                {
                    var fileInfo = (decimal)(new FileInfo(fullPath).Length) / 1000000;
                    body += "FileSize: " + fileInfo + " MB" + Environment.NewLine;
                }
                
                body += "DriveLetter: " + Path.GetPathRoot(fullPath) + Environment.NewLine;
            }

            var users = Utils.GetCurrentUsers();
            body += "CurrentLoggedInUsers: " + string.Join(" ", users.UserNames);
            
            var entry = new Event
            {              
                DriveLetter = diskLetter,
                Action = action,
                EventId = id,
                EventType = EventLogEntryType.SuccessAudit,
                EventBody = body,
                //LogonIds = users.LogonIds,
                //UserNames = users.UserNames
            };

            Logger.WriteLog(entry);
        }
    }
}