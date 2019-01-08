using System.ServiceProcess;

namespace UsbWatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new UsbWatcherService(), 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
