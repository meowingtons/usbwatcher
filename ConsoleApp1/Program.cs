using System.Threading;
using UsbWatcher.Watchers;
using USBLogging;

namespace ConsoleApp1
{
    class Program
    {
        //use this project to launch the same processes the service does, but with a console so it's easier to debug
        static void Main(string[] args)
        {
            Utils.ConfigureEventLog();
            DriveWatcher.FindExistingDrives();

            var _watcher = new DriveWatcher();
            _watcher.StartWatching();

            Thread.Sleep(100000000);
        }
    }
}
