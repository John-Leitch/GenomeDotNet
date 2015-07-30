using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ConsoleServerComponent : ServiceComponent<ObjectServer>
    {
        public int Timeout { get; set; }

        public ConsoleServerComponent()
        {
            Timeout = 10000;
            Timeout = 100000;
        }

        public ConsoleMessage ExpectConsoleMessage(ClientContext context, ConsoleMessageType type)
        {
            return Network.Expect<ConsoleMessage>(context, x => x.Type == type, Timeout);
        }

        public ConsoleMessage ExpectConsoleMessage(ClientContext context, ConsoleMessageType type, Guid id)
        {
            return Network.Expect<ConsoleMessage>(context, x => x.Type == type && x.Id == id, Timeout);
        }

        public void WriteConsoleMessage(ClientContext context, ConsoleMessageType type)
        {
            Network.WriteObject(context, new ConsoleMessage(type));
        }

        public void WriteConsoleMessage(ClientContext context, ConsoleMessageType type, Guid id)
        {
            Network.WriteObject(context, new ConsoleMessage(type, id));
        }

        public void WriteConsoleMessage(ClientContext context, ConsoleMessageType type, Guid id, string data)
        {
            Network.WriteObject(context, new ConsoleMessage(type, id, data));
        }

        public string ExecuteScript(ClientContext context, string script)
        {
            WriteConsoleMessage(context, ConsoleMessageType.Register);
            var resp = ExpectConsoleMessage(context, ConsoleMessageType.Register);
            var id = resp.Id;
            WriteConsoleMessage(context, ConsoleMessageType.Write, id, script);
            resp = ExpectConsoleMessage(context, ConsoleMessageType.Write);

            var respStr = new StringBuilder();

            do
            {
                WriteConsoleMessage(context, ConsoleMessageType.Read, id);
                resp = ExpectConsoleMessage(context, ConsoleMessageType.Read);

                if (resp.Data != null)
                {
                    respStr.Append(resp.Data);
                    Thread.Sleep(200);
                }
            } while (resp.Data != null);

            WriteConsoleMessage(context, ConsoleMessageType.Unregister, id);
            resp = ExpectConsoleMessage(context, ConsoleMessageType.Unregister);

            return respStr.ToString();
        }

        public RemoteConsole CreateConsole(ClientContext context)
        {
            WriteConsoleMessage(context, ConsoleMessageType.Register);
            var msg = ExpectConsoleMessage(context, ConsoleMessageType.Register);

            return new RemoteConsole(this, context, msg.Id);
        }

        protected override void RegisterCore(ObjectServer network)
        {
            network.RegisterType<ConsoleMessage>();
        }
    }
}
