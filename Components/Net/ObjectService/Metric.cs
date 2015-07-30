using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    [Serializable]
    public class Metric
    {
        public Guid Id { get; set; }

        public string Machine { get; set; }

        public string Name { get; set; }

        public int Iterations { get; set; }

        //public int Exceptions { get; set; }

        public int RunTime { get; set; }

        public int Threads { get; set; }

        public Dictionary<string, int> Exceptions { get; set; }
    }

    [Serializable]
    public enum MetricValueType
    {
        Int,
        String,
        Bool,
    }

    [Serializable]
    public class MetricValue
    {
        public MetricValueType Type { get; set; }

        public long Int { get; set; }

        public string String { get; set; }

        public bool Bool { get; set; }

        public MetricValue()
        {
        }

        public MetricValue(MetricValueType type)
            : this()
        {
            Type = type;
        }

        public MetricValue(long value)
            : this(MetricValueType.Int)
        {
            Int = value;
        }

        public MetricValue(string value)
            : this(MetricValueType.String)
        {
            String = value;
        }

        public MetricValue(bool value)
            : this(MetricValueType.Bool)
        {
            Bool = value;
        }
    }
}
