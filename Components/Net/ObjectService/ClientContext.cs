using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public sealed class ClientContext : IDisposable
    {
        private object _readSync = new object(), _writeSync = new object();

        public Guid Id { get; private set; }

        public TcpClient Client { get; private set; }

        public NetworkStream Stream { get; private set; }

        public NetworkStreamWriter Writer { get; private set; }

        public NetworkStreamReader Reader { get; private set; }

        public bool DisconnectEventsTriggered { get; set; }

        public ClientContext(
            TcpClient client, 
            NetworkStream stream, 
            NetworkStreamWriter writer, 
            NetworkStreamReader reader)
        {
            Id = Guid.NewGuid();
            Client = client;
            Stream = stream;
            Writer = writer;
            Reader = reader;
        }

        public static ClientContext FromClient(TcpClient client)
        {
            var s = client.GetStream();

            return new ClientContext(
                client, 
                s, 
                new NetworkStreamWriter(s), 
                new NetworkStreamReader(s));
        }

        public void Dispose()
        {
            if (Stream != null)
            {
                Stream.Dispose();
            }

            if (Writer != null)
            {
                Writer.Dispose();
            }

            if (Reader != null)
            {
                Reader.Dispose();
            }

            if (Client != null)
            {
                Client.Close();
            }
        }

        public void AcquireReadLock()
        {
            Monitor.Enter(_readSync);
        }

        public void ReleaseReadLock()
        {
            Monitor.Exit(_readSync);
        }

        public void AcquireWriteLock()
        {
            Monitor.Enter(_writeSync);
        }

        public void ReleaseWriteLock()
        {
            Monitor.Exit(_writeSync);
        }
    }
}
