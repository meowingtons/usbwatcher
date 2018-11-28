using System;
using System.IO;
using System.Security.Permissions;
using System.Diagnostics;

namespace USBLogging
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
            _watcher.Created += new FileSystemEventHandler(OnChanged);
            _watcher.Deleted += new FileSystemEventHandler(OnChanged);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed);
            _watcher.Error += new ErrorEventHandler(OnError);

            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
            Console.WriteLine("Watching Drive: " + driveLetter);
        }

        public static void Stop(string driveLetter)
        {
            _watcher.Dispose();
            Console.WriteLine("Watcher stopped on drive: " + driveLetter);
        } 
            
        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("File: " +  e.FullPath + " " + e.ChangeType);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        private static void OnError(object source, ErrorEventArgs e)
        {
            Console.WriteLine("File: " +  e.GetException());
        }
    }
}