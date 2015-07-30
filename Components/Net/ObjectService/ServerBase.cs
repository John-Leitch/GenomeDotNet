using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public abstract class ServerBase : NetworkComponent
    {
        private enum SessionState
        {
            WaitingForHello,
            Initialized,
        }

        public bool IsListening { get; private set; }

        private Thread _listenThread;

        public TcpListener Listener { get; private set; }

        public event EventHandler Listening;

        private List<ClientContext> _clients = new List<ClientContext>();

        public ServerBase()
        {

        }

        public ClientContext[] GetClients()
        {
            lock (_clients)
            {
                return _clients.ToArray();
            }
        }

        private void HandleClientHello(ClientContext context, Message message)
        {
            switch (message.Type)
            {
                case MessageType.Hello:
                    InvokeHelloReceived(context.Client);
                    InvokeConnecting(context.Client);
                    context.AcquireWriteLock();

                    try
                    {
                        context.Writer.WriteHello();
                    }
                    finally
                    {
                        context.ReleaseWriteLock();
                    }

                    InvokeHelloSent(context.Client);

                    lock (_clients)
                    {
                        _clients.Add(context);
                    }

                    InvokeConnected(context.Client);

                    break;

                default:
                    context.AcquireWriteLock();

                    try
                    {
                        context.Writer.WriteError("Hello expected");
                    }
                    finally
                    {
                        context.ReleaseWriteLock();
                    }

                    break;
            }
        }

        private void HandleGoodbyeMessage(ClientContext context)
        {
            InvokeGoodbyeReceived(context.Client);
            InvokeDisconnecting(context.Client);
            context.AcquireWriteLock();

            try
            {
                context.Writer.WriteGoodbye();
            }
            finally
            {
                context.ReleaseWriteLock();
            }

            InvokeGoodbyeSent(context.Client);
            InvokeDisconnected(context.Client);
            context.DisconnectEventsTriggered = true;
            context.Client.Close();
            //context.IsConnected = false;
            
            lock (_clients)
            {
                _clients.Remove(context);
            }
        }

        private void HandleArbitraryMessage(ClientContext context, Message message)
        {
            switch (message.Type)
            {
                case MessageType.Data:
                    HandleData(context, message.Data);
                    break;

                case MessageType.Goodbye:
                    HandleGoodbyeMessage(context);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        private void HandleClient(ClientContext context)
        {
            var state = SessionState.WaitingForHello;
            
            using (context)
            {
                while (context.Client.Connected)
                {
                    context.AcquireReadLock();

                    Message m = null;

                    try
                    {
                        m = context.Reader.ReadMessage();
                    }
                    catch (Exception e)
                    {
                        InvokeClientErrorEncountered(new TcpClientError(context.Client, e));
                    }

                    if (m == null)
                    {
                        context.ReleaseReadLock();
                        Thread.Sleep(1);
                        continue;
                    }

                    switch (state)
                    {
                        case SessionState.WaitingForHello:
                            HandleClientHello(context, m);
                            state = SessionState.Initialized;
                            break;

                        case SessionState.Initialized:
                            HandleArbitraryMessage(context, m);
                            break;

                        default:
                            throw new InvalidOperationException();
                    }

                    context.ReleaseReadLock();
                }

                if (!context.DisconnectEventsTriggered)
                {
                    InvokeDisconnecting(context.Client);
                    InvokeDisconnected(context.Client);

                    lock (_clients)
                    {
                        _clients.Remove(context);
                    }
                }
            }
        }

        public void Listen()
        {
            if (IsListening || Listener != null)
            {
                throw new InvalidOperationException();
            }

            IsListening = true;
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start();

            if (Listening != null)
            {
                Listening(this, new EventArgs());
            }

            while (IsListening)
            {
                TcpClient client;

                try
                {
                    client = Listener.AcceptTcpClient();
                }
                catch (SocketException)
                {
                    continue;
                }

                var context = ClientContext.FromClient(client);

                ThreadHelper.DoBackground(() =>
                {
                    using (client)
                    {
                        HandleClient(context);
                    }
                });
            }
        }

        public void ListenAsync()
        {
            _listenThread = ThreadHelper.DoBackground(Listen);
        }

        public void Stop()
        {
            if (!IsListening || _listenThread == null)
            {
                throw new InvalidOperationException();
            }

            IsListening = false;
            Listener.Stop();
            _listenThread.TryKill();
        }

        public override void Dispose()
        {
            if (Listener != null)
            {
                IsListening = false;
                Listener.Stop();                
            }
        }
    }
}
