using Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public abstract class ClientBase : NetworkComponent
    {
        private enum ClientState
        {
            Waiting,
            WaitingForGoodbye,
            Disconnected,
        }

        private ClientState _state = ClientState.Waiting;

        public bool IsConnected { get; private set; }

        public event ItemEventHandler<Exception> ConnectFailed;

        public event ItemEventHandler<Message> MessageReceived;

        public event ItemEventHandler<byte[]> MessageSent;

        public ClientContext Context { get; private set; }

        public Thread ReadThread { get; private set; }

        private ManualResetEvent _goodbyeReset = new ManualResetEvent(false);

        private void HandleMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.Data:
                    HandleData(Context, message.Data);
                    break;

                case MessageType.Goodbye:
                    _state = ClientState.Disconnected;
                    _goodbyeReset.Set();
                    break;
            }
        }

        private Thread CreateReadThread()
        {
            return ThreadHelper.DoBackground(() =>
            {
                while (Context.Client.Connected)
                {
                    Context.AcquireReadLock();
                    Message message = null;

                    try
                    {
                        message = Context.Reader.ReadMessage();
                    }
                    catch (Exception e)
                    {
                        InvokeClientErrorEncountered(new TcpClientError(Context.Client, e));
                    }

                    if (message == null)
                    {
                        Context.ReleaseReadLock();
                        Thread.Sleep(1);

                        try
                        {
                            if (Context.Client.Client.Poll(0, SelectMode.SelectRead) &&
                                Context.Client.Client.Receive(new byte[1], SocketFlags.Peek) == 0)
                            {
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        catch
                        {
                            break;
                        }
                    }

                    if (MessageReceived != null)
                    {
                        MessageReceived(this, new ItemEventArgs<Message>(message));
                    }

                    HandleMessage(message);
                    Context.ReleaseReadLock();
                }

                IsConnected = false;
                InvokeDisconnecting(Context.Client);
                InvokeDisconnected(Context.Client);
            });
        }

        public void Connect(string hostname)
        {
            if (IsConnected)
            {
                throw new InvalidOperationException();
            }

            do
            {
                var c = new TcpClient();
                InvokeConnecting(c);

                try
                {
                    c.Connect(hostname, Port);
                    Context = ClientContext.FromClient(c);
                    Context.AcquireWriteLock();
                    Context.AcquireReadLock();

                    try
                    {
                        Context.Writer.WriteHello();
                        InvokeHelloSent(Context.Client);
                        Context.Reader.ReadHello();
                    }
                    finally
                    {
                        Context.ReleaseWriteLock();
                        Context.ReleaseReadLock();
                    }
                    
                    InvokeHelloReceived(Context.Client);
                    ReadThread = CreateReadThread();
                    IsConnected = true;
                }
                catch (Exception e)
                {
                    if (ConnectFailed != null)
                    {
                        ConnectFailed(this, new ItemEventArgs<Exception>(e));
                    }

                    try
                    {
                        c.Client.Dispose();
                    }
                    catch { }

                    try
                    {
                        c.Client.Shutdown(SocketShutdown.Both);
                    }
                    catch { }

                    try
                    {
                        c.Close();
                    }
                    catch { }
                    
                    Thread.Sleep(1000);
                }
            } while (!IsConnected);

            InvokeConnected(Context.Client);
        }

        public void Disconnect()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException();
            }

            Context.AcquireWriteLock();

            try
            {
                _goodbyeReset.Reset();
                _state = ClientState.WaitingForGoodbye;
                Context.Writer.WriteGoodbye();
                InvokeGoodbyeSent(Context.Client);
                _goodbyeReset.WaitOne();
                IsConnected = false;
                ReadThread.TryKill();
                InvokeGoodbyeReceived(Context.Client);
            }
            finally
            {
                Context.ReleaseWriteLock();
            }
        }

        public override void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
            }

            if (ReadThread != null)
            {
                ReadThread.TryKill();
            }

            if (_goodbyeReset != null)
            {
                _goodbyeReset.Dispose();
            }
        }

        public void WriteData(byte[] data)
        {
            Context.AcquireWriteLock();

            try
            {
                Context.Writer.WriteData(data);
            }
            finally
            {
                Context.ReleaseWriteLock();
            }

            if (MessageSent != null)
            {
                MessageSent(this, data);
            }
        }

        public void WriteData(string data)
        {
            Context.AcquireWriteLock();

            try
            {
                Context.Writer.WriteData(data);
            }
            finally
            {
                Context.ReleaseWriteLock();
            }

            if (MessageSent != null)
            {
                MessageSent(this, data.GetBytes());
            }
        }
    }
}
