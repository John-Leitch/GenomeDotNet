using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public interface IClientScriptExecutor
    {
        BatchScriptCommand[] Execute(ClientContext context, string script);
    }
}
