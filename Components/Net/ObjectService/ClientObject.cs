using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ClientObject<TMessage>
    {
        public ClientContext Context { get; set; }

        public TMessage Object { get; set; }

        public ClientObject(ClientContext context, TMessage obj)
        {
            Context = context;
            Object = obj;
        }
    }
}
