namespace SystemMonitoringDashboard.Models
{
    public class SystemStats
    {
        public double CpuUsage { get; set; }

        public double MemoryUsage { get; set; }

        public double DiskUsage { get; set; }

        public int ProcessCount { get; set; }
    }
}