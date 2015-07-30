using Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public sealed class NetworkStreamReader : IDisposable
    {
        private enum StreamReaderState
        {
            ReadingMessageType,
            ReadingMessageLength,
            ReadingMessageData,
        }

        public Stream Stream { get; private set; }

        public NetworkStreamReader(Stream stream)
        {
            Stream = stream;
        }

        public Message ReadMessage()
        {
            var state = StreamReaderState.ReadingMessageType;

            while (true)
            {
                switch (state)
                {
                    case StreamReaderState.ReadingMessageType:
                        var m = (MessageType)Stream.ReadByte();

                        switch (m)
                        {
                            case MessageType.Hello:
                            case MessageType.Goodbye:
                                return new Message(m);

                            case MessageType.NoData:
                                return null;

                            case MessageType.Data:
                                var len = BitConverter.ToInt32(Stream.Read(4), 0);

                                var data = Stream.Read(len);
                                var remaining = len - data.Length;

                                while (remaining > 0)
                                {
                                    var data2 = Stream.Read(remaining);
                                    data = data.Concat(data2).ToArray();
                                    remaining -= data2.Length;
                                }

                                if (data.Length != len)
                                {
                                    throw new InvalidOperationException();
                                }

                                return new Message(MessageType.Data, len, data);

                            default:
                                throw new NotImplementedException();
                        }

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public Message ReadBodylessMessage(MessageType type, int errorCode)
        {
            var message = ReadMessage();

            if (message.Type != type)
            {
                throw new SocketException(errorCode);
            }
            else
            {
                return message;
            }
        }

        public Message ReadHello()
        {
            return ReadBodylessMessage(MessageType.Hello, 111);
        }

        public Message ReadGoodbye()
        {
            return ReadBodylessMessage(MessageType.Goodbye, 112);
        }

        public void Dispose()
        {
            if (Stream != null)
            {
                Stream.Dispose();
            }
        }
    }
}
