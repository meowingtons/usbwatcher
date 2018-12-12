using System;
using System.Management;
// ReSharper disable MemberCanBePrivate.Global

namespace USBLogging
{
    public class DriveProperties
    {
        //these properties MUST be publicly scoped or other shit breaks. Ignore resharper.
        public string DriveLetter { get; set; }
        public string DeviceId { get; set; }
        public string SerialNumber { get; set; }
        public string DeviceModel { get; set; }
        public double Size { get; set; }
        public string InterfaceType { get; set; }

        //of course windows does this fucking stupidly, so we have to search for logical disks first. From there,
        //we can get the associated partitions. Once we have the partitions, we can try to find the disk(s) those
        //partitions reside on. Once we have the disk, we can grab all the pertinent properties and return them.
        //NOTE: THIS DOES NOT WORK WITH SOFTWARE RAID WHERE PARTITIONS SPAN MULTIPLE DISKS!!!!!!
        //(honestly though, who the fuck is setting up software RAID with USB sticks??)
        public static DriveProperties GetDeviceProperties(string driveLetter)
        {          
            using (var partitions = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" +
                                                                 driveLetter +
                                                                 "'} WHERE ResultClass=Win32_DiskPartition"))
            {
                var partitionsResult = partitions.Get();
                
                if (partitionsResult.Count != 1) throw new Exception("Error searching for partitions with drive letter: " + driveLetter);
                
                foreach (var partition in partitionsResult)
                {
                    using (var disks = new ManagementObjectSearcher(
                        "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" +
                        partition["DeviceID"] +
                        "'} WHERE ResultClass=Win32_DiskDrive"))
                    {
                        var result = disks.Get();

                        if (result.Count != 1)
                            throw new Exception(
                                "Multiple disks found for single partition. Something weird happened.");
                       
                        
                        var drive = new DriveProperties();

                        foreach (var disk in result)
                        {
                            drive.Size = (double)((ulong)disk.Properties["Size"].Value)/1000000000;
                            drive.DeviceId = (string)disk.Properties["DeviceID"].Value;
                            drive.DeviceModel = (string)disk.Properties["Model"].Value;
                            drive.DriveLetter = driveLetter;
                            drive.InterfaceType = (string)disk.Properties["InterfaceType"].Value;
                            drive.SerialNumber = (string)disk.Properties["SerialNumber"].Value;
                        }
                        
                        return drive;
                    }
                }
            }
            
            //we should never get here but just in case
            throw new Exception("Error Searching for Partitions on drive letter: " + driveLetter);
        }
    }
}