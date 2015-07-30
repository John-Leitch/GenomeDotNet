using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class BatchScriptTemplate
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public List<ParameterSet> ParameterSets { get; set; }

        public string Populate(ParameterSet parameterSet)
        {
            var t = new StringTemplate()
            {
                Text = Text,
            };

            return t.Populate(parameterSet.Parameters);
        }
    }
}
