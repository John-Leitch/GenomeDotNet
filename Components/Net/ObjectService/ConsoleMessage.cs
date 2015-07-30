using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Net.ObjectService
{
    public class ConsoleMessage
    {
        public ConsoleMessageType Type { get; set; }

        public Guid Id { get; set; }

        public string Data { get; set; }

        public ConsoleMessage(ConsoleMessageType type, Guid id, string data)
        {
            Type = type;
            Data = data;
            Id = id;
        }

        public ConsoleMessage(ConsoleMessageType type, Guid id)
            : this(type, id, null)
        {
        }

        public ConsoleMessage(ConsoleMessageType type)
            : this(type, default(Guid))
        {
        }

        public ConsoleMessage()
            : this(default(ConsoleMessageType))
        {
        }
    }
}
