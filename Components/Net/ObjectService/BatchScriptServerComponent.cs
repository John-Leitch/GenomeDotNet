using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class BatchScriptServerComponent : ConsoleServerComponent, IClientScriptExecutor
    {
        public event EventHandler<BatchScriptExecutionEventArgs> CreatingConsole, ConsoleCreated, CommandExecuting, CommandExecuted;

        public BatchScriptCommand[] Execute(ClientContext context, string script)
        {
            var lines = script.SplitLines(StringSplitOptions.RemoveEmptyEntries);
            var results = new BatchScriptCommand[lines.Length];

            if (CreatingConsole != null)
            {
                CreatingConsole(this, new BatchScriptExecutionEventArgs(context, null));
            }

            using (var client = CreateConsole(context))
            {
                if (ConsoleCreated != null)
                {
                    ConsoleCreated(this, new BatchScriptExecutionEventArgs(context, null));
                }

                client.ReadResponse();

                for (int i = 0; i < lines.Length; i++)
                {
                    if (CommandExecuting != null)
                    {
                        CommandExecuting(this, new BatchScriptExecutionEventArgs(context, new BatchScriptCommand(lines[i], null)));
                    }

                    results[i] = new BatchScriptCommand(lines[i], client.ExecuteCommand(lines[i]));

                    if (CommandExecuted != null)
                    {
                        CommandExecuted(this, new BatchScriptExecutionEventArgs(context, results[i]));
                    }
                }
            }

            return results;
        }
    }
}
