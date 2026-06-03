using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using SystemMonitoringDashboard.Services;

namespace SystemMonitoringDashboard
{
    public partial class MainWindow : Window
    {
        private readonly SystemMonitorService monitorService;
        private readonly DispatcherTimer timer;

        private readonly ObservableCollection<double> cpuValues = new();
        private readonly ObservableCollection<double> memoryValues = new();
        private readonly ObservableCollection<double> diskValues = new();

        public ISeries[] Series { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            monitorService = new SystemMonitorService();

            Series =
            [
                new LineSeries<double>
                {
                    Values = cpuValues,
                    Name = "CPU %"
                },
                new LineSeries<double>
                {
                    Values = memoryValues,
                    Name = "Memory %"
                },
                new LineSeries<double>
                {
                    Values = diskValues,
                    Name = "Disk %"
                }
            ];

            UsageChart.Series = Series;

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            timer.Tick += Timer_Tick;
            timer.Start();

            UpdateStats();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateStats();
        }

        private void UpdateStats()
        {
            var stats = monitorService.GetSystemStats();

            CpuText.Text = $"{stats.CpuUsage}%";
            MemoryText.Text = $"{stats.MemoryUsage}%";
            DiskText.Text = $"{stats.DiskUsage}%";
            ProcessText.Text = stats.ProcessCount.ToString();

            AddValue(cpuValues, stats.CpuUsage);
            AddValue(memoryValues, stats.MemoryUsage);
            AddValue(diskValues, stats.DiskUsage);
        }

        private void AddValue(ObservableCollection<double> collection, double value)
        {
            collection.Add(value);

            if (collection.Count > 20)
            {
                collection.RemoveAt(0);
            }
        }
    }
}