using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class BatchScriptRun
    {
        public BatchScriptTemplate Template { get; set; }

        public ParameterSet ParameterSet { get; set; }

        public BatchScriptCommand Command { get; set; }
    }
}
