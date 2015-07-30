using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class BatchScriptExecutionEventArgs : EventArgs
    {
        public ClientContext Context { get; private set; }

        public BatchScriptCommand Command { get; private set; }

        public BatchScriptExecutionEventArgs(ClientContext context, BatchScriptCommand command)
        {
            Context = context;
            Command = command;
        }
    }
}
