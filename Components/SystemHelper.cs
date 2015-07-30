using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class SystemHelper
    {
        private static PerformanceCounter _cpuCounter = new PerformanceCounter(
                "Processor Information",
                "% Processor Time",
                "_Total");

        private static PerformanceCounter _memoryCounter = new PerformanceCounter(
            "Memory",
            "% Committed Bytes In Use");

        public static float GetCpuUsage()
        {
            return _cpuCounter.NextValue();
        }

        public static float GetMemoryUsage()
        {
            return _memoryCounter.NextValue();
        }
    }
}
