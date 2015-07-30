using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class Message
    {
        public MessageType Type { get; private set; }

        public int Length { get; private set; }

        public byte[] Data { get; private set; }

        public Message(MessageType type, int length, byte[] data)
        {
            Type = type;
            Length = length;
            Data = data;
        }

        public Message(MessageType type)
            : this(type, 0, null)
        {
        }
    }
}
