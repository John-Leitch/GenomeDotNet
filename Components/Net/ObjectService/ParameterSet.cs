using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ParameterSet
    {
        public string Name { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public ParameterSet()
        {
        }

        public ParameterSet(string name, Dictionary<string, string> parameters)
            : this()
        {
            Name = name;
            Parameters = parameters;
        }
    }
}
