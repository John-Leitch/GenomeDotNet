using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class SystemInfo : Bindable
    {
        private DateTime _dateTime;

        public DateTime DateTime
        {
            get { return _dateTime; }
            set 
            {
                _dateTime = value;
                InvokePropertyChanged();
            }
        }

        private string _machineName;

        public string MachineName
        {
            get { return _machineName; }
            set 
            {
                _machineName = value;
                InvokePropertyChanged();
            }
        }

        private float _cpuUsage;

        public float CpuUsage
        {
            get { return _cpuUsage; }
            set 
            {
                _cpuUsage = value;
                InvokePropertyChanged();
            }
        }

        private float _memoryUsage;

        public float MemoryUsage
        {
            get { return _memoryUsage; }
            set 
            {
                _memoryUsage = value;
                InvokePropertyChanged();
            }
        }

        public static SystemInfo Create()
        {
            var categories = PerformanceCounterCategory
                .GetCategories()
                .Where(x => x.CategoryName.ToLower().Contains("mem"))
                .Select(x => x.CategoryName)
                .ToArray();

            var info = new SystemInfo()
            {
                MachineName = Environment.MachineName,
                CpuUsage = SystemHelper.GetCpuUsage(),
                MemoryUsage = SystemHelper.GetMemoryUsage(),
                DateTime = DateTime.Now,
            };

            return info;
        }
    }
}
