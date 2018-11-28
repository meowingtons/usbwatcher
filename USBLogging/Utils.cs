using System.Collections.Generic;
using System.Management;

namespace USBLogging
{
    public static class Utils
    {
        public static List<string> GetCurrentUsers()
        {
            var users = new List<string>();
            
            var searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
            var result = searcher.Get();
            
            foreach (var user in result)
            {
                users.Add(user["UserName"].ToString());
            }

            return users;
        }
    }
}