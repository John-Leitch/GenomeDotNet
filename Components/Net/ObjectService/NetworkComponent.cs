using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public abstract class NetworkComponent : IDisposable
    {
        public event ItemEventHandler<TcpClient> HelloReceived;

        public event ItemEventHandler<TcpClient> HelloSent;

        public event ItemEventHandler<TcpClient> GoodbyeReceived;

        public event ItemEventHandler<TcpClient> GoodbyeSent;

        public event ItemEventHandler<TcpClient> Connecting;

        public event ItemEventHandler<TcpClient> Connected;

        public event ItemEventHandler<TcpClient> Disconnecting;

        public event ItemEventHandler<TcpClient> Disconnected;

        public event ItemEventHandler<TcpClientError> ClientErrorEncountered;

        public int Port { get; set; }

        public NetworkComponent()
        {
            Port = 5230;
        }

        public abstract void Dispose();

        protected void InvokeConnecting(TcpClient client)
        {
            if (Connecting != null)
            {
                Connecting(this, client);
            }
        }

        protected void InvokeConnected(TcpClient client)
        {
            if (Connected != null)
            {
                Connected(this, client);
            }
        }

        protected void InvokeHelloReceived(TcpClient client)
        {
            if (HelloReceived != null)
            {
                HelloReceived(this, client);
            }
        }

        protected void InvokeHelloSent(TcpClient client)
        {
            if (HelloSent != null)
            {
                HelloSent(this, client);
            }
        }

        protected void InvokeGoodbyeReceived(TcpClient client)
        {
            if (GoodbyeReceived != null)
            {
                GoodbyeReceived(this, client);
            }
        }

        protected void InvokeGoodbyeSent(TcpClient client)
        {
            if (GoodbyeSent != null)
            {
                GoodbyeSent(this, client);
            }
        }

        protected void InvokeDisconnecting(TcpClient client)
        {
            if (Disconnecting != null)
            {
                Disconnecting(this, client);
            }
        }

        protected void InvokeDisconnected(TcpClient client)
        {
            if (Disconnected != null)
            {
                Disconnected(this, client);
            }
        }

        protected void InvokeClientErrorEncountered(TcpClientError error)
        {
            if (ClientErrorEncountered != null)
            {
                ClientErrorEncountered(this, error);
            }
        }

        protected abstract void HandleData(ClientContext context, byte[] data);
    }
}
