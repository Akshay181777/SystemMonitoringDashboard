using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using SystemMonitoringDashboard.Models;

namespace SystemMonitoringDashboard.Services
{
    public class SystemMonitorService
    {
        private readonly PerformanceCounter cpuCounter;

        public SystemMonitorService()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
        }

        public SystemStats GetSystemStats()
        {
            return new SystemStats
            {
                CpuUsage = Math.Round(cpuCounter.NextValue(), 2),
                MemoryUsage = GetMemoryUsage(),
                DiskUsage = GetDiskUsage(),
                ProcessCount = Process.GetProcesses().Length
            };
        }

        private double GetMemoryUsage()
        {
            ObjectQuery query = new ObjectQuery("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");

            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            foreach (ManagementObject result in searcher.Get())
            {
                double totalMemory = Convert.ToDouble(result["TotalVisibleMemorySize"]);
                double freeMemory = Convert.ToDouble(result["FreePhysicalMemory"]);
                double usedMemory = totalMemory - freeMemory;

                return Math.Round((usedMemory / totalMemory) * 100, 2);
            }

            return 0;
        }

        private double GetDiskUsage()
        {
            DriveInfo drive = DriveInfo.GetDrives()
                .FirstOrDefault(d => d.Name == "C:\\")!;

            if (drive == null)
                return 0;

            double usedSpace = drive.TotalSize - drive.AvailableFreeSpace;

            return Math.Round((usedSpace / drive.TotalSize) * 100, 2);
        }
    }
}