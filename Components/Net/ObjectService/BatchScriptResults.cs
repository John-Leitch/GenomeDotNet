using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class BatchScriptResults
    {
        public string Text { get; set; }

        public Dictionary<string, BatchScriptCommand[]> Machines { get; set; }

        public BatchScriptResults()
        {
        }

        public BatchScriptResults(string text, Dictionary<string, BatchScriptCommand[]> machines)
            : this()
        {
            Text = text;
            Machines = machines;
        }
    }
}
