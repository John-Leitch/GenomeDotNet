using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class UnexpectedMessageException : Exception
    {
        public MessageType MessageType { get; set; }

        public MessageType ExpectedMessageType { get; set; }

        public UnexpectedMessageException(
            MessageType messageType,
            MessageType expectedMessageType)
        {
            MessageType = messageType;
            ExpectedMessageType = expectedMessageType;
        }
    }
}
