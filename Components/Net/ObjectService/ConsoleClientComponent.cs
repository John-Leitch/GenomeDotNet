using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ConsoleClientComponent : ServiceComponent<ObjectClient>
    {
        private ConsoleService _consoleService = new ConsoleService();

        protected override void RegisterCore(ObjectClient network)
        {
            network.RegisterType<ConsoleMessage>(ConsoleMessageReceived);
        }

        public bool ConsoleMessageReceived(ConsoleMessage message)
        {
            switch (message.Type)
            {
                case ConsoleMessageType.Register:
                    var guid = _consoleService.Register();
                    Network.WriteObject(new ConsoleMessage(ConsoleMessageType.Register, guid));
                    break;

                case ConsoleMessageType.Write:
                    _consoleService.WriteBuffer(message.Id, message.Data);
                    Network.WriteObject(new ConsoleMessage(ConsoleMessageType.Write, message.Id));
                    break;

                case ConsoleMessageType.Read:
                    var data = _consoleService.ReadBuffer(message.Id);
                    Network.WriteObject(new ConsoleMessage(ConsoleMessageType.Read, message.Id, data));
                    break;

                case ConsoleMessageType.Unregister:
                    _consoleService.Unregister(message.Id);
                    Network.WriteObject(new ConsoleMessage(ConsoleMessageType.Unregister, message.Id));
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return true;
        }
    }
}
