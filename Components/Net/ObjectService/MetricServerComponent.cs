using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class MetricServerComponent : ObjectDatabaseServerComponent<Metric, Guid>
    {
        public MetricServerComponent()
            : base(x => x.Id)
        {
        }
    }
}
