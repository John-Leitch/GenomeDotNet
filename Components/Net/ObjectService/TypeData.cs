using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class TypeData
    {
        public ClientContext Context { get; set; }

        public string[] Parts { get; set; }

        public bool IsHandled { get; set; }

        public TypeData(ClientContext context, string[] parts)
        {
            Context = context;
            Parts = parts;
        }
    }
}
