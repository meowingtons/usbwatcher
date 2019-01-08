using System.ComponentModel;

namespace usbwatcher
{
    [RunInstaller(true)]
    public partial class UsbWatcherServiceInstaller : System.Configuration.Install.Installer
    {
        public UsbWatcherServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
