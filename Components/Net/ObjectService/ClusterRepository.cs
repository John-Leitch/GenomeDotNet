using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ClusterRepository : JsonRepository<ClusterInfo>
    {
        protected override string GetScriptFile()
        {
            return PathHelper.GetExecutingPath("machines.json");
        }
    }
}
