using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerBlockChain.Entities
{
    public class HardwareInfomation
    {
        public double Temperature { get; set; }
        public double CpuUsage { get; set; }
        public double GpuUsage { get; set; }
        public double RamUsage { get; set; }
        public double DiskUsage { get; set; }
        public int FanSpeed { get; set; }

        public string ProcessorName { get; set; } = string.Empty;
        public string TotalRAM { get; set; } = string.Empty;
        public string TotalDiskSpace { get; set; } = string.Empty;
    }
}