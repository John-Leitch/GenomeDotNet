using Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class NetworkStreamWriter : IDisposable
    {
        public Stream Stream { get; private set; }

        public NetworkStreamWriter(Stream stream)
        {
            Stream = stream;
        }

        public void Dispose()
        {
            if (Stream != null)
            {
                Stream.Dispose();
            }
        }

        public void WriteHello()
        {
            Stream.WriteByte((byte)MessageType.Hello);
        }

        public void WriteGoodbye()
        {
            Stream.WriteByte((byte)MessageType.Goodbye);
        }

        private void WriteBodiedMessage(MessageType type, byte[] data)
        {
            Stream.WriteByte((byte)type);
            Stream.Write(BitConverter.GetBytes(data.Length));
            Stream.Write(data);
        }

        private void WriteBodiedMessage(MessageType type, string data)
        {
            WriteBodiedMessage(type, data.GetBytes());
        }

        public void WriteData(byte[] data)
        {
            WriteBodiedMessage(MessageType.Data, data);
        }

        public void WriteData(string data)
        {
            WriteBodiedMessage(MessageType.Data, data);
        }

        public void WriteError(byte[] data)
        {
            WriteBodiedMessage(MessageType.Error, data);
        }

        public void WriteError(string data)
        {
            WriteBodiedMessage(MessageType.Error, data);
        }
    }
}
