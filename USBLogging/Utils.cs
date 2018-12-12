using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace USBLogging
{
    public static class Utils
    {
        public static CurrentUsers GetCurrentUsers()
        {
            var users = new CurrentUsers();
            users.LogonIds = new List<string>();
            users.UserNames = new List<string>();
            
            try
            {
                var query = new ObjectQuery("SELECT LogonId FROM Win32_LogonSession Where LogonType=2");
                var searcher = new ManagementObjectSearcher(query);
                var objects = searcher.Get();

                foreach (var o in objects)
                {
                    var wmiObject = (ManagementObject) o;
                    var lQuery = new ObjectQuery("Associators of {Win32_LogonSession.LogonId=" + wmiObject["LogonId"] + "} Where AssocClass=Win32_LoggedOnUser Role=Dependent");
                    var lSearcher = new ManagementObjectSearcher(lQuery);
                    var lResult = lSearcher.Get();

                    foreach (var managementBaseObject in lResult)
                    {
                        var lWmiObject = (ManagementObject) managementBaseObject;
                        var username = lWmiObject["Name"].ToString();
    
                        if (!users.UserNames.Contains(username))
                        {
                            users.UserNames.Add(username);
                        }                        
                    }
                }

                return users;
            }
            catch (Exception e)
            {
                throw new Exception("Error getting current users.");
            }
        }
    }
}