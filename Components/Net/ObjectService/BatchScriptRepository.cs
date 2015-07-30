using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class BatchScriptRepository : JsonRepository<BatchScriptTemplate[]>
    {
        protected override string GetScriptFile()
        {
            return PathHelper.GetExecutingPath("scripts.json");
        }
    }
}
