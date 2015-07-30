using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class MetricClient : ObjectDatabaseClient<Metric, Guid>
    {
        public MetricClient(ObjectClient client)
            : base(client, x => x.Id)
        {
        }
    }
}
