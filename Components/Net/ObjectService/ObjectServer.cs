using Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ObjectServer<TMessage> : ServerBase
    {
        public event ItemEventHandler<ClientObject<TMessage>> ObjectReceived;

        protected override void HandleData(ClientContext context, byte[] data)
        {
            var o = JsonSerializer.Deserialize<TMessage>(data.GetString());

            if (ObjectReceived != null)
            {
                ObjectReceived(this, new ClientObject<TMessage>(context, o));
            }
        }

        public void WriteObject(ClientContext context, TMessage obj)
        {
            var s = JsonSerializer.Serialize(obj);
            context.AcquireWriteLock();

            try
            {
                context.Writer.WriteData(s);
            }
            finally
            {
                context.ReleaseWriteLock();
            }
        }
    }

    public class ObjectServer : ServerBase
    {
        public event ItemEventHandler<Tuple<ClientContext, byte[]>> DataSent, DataReceived;

        private List<byte[]> _unhandledMessages = new List<byte[]>();

        private TypeCallbackCollection _callbacks = new TypeCallbackCollection();

        private ExpectationTable _expectations = new ExpectationTable();

        private List<ServiceComponent<ObjectServer>> _serviceComponents = new List<ServiceComponent<ObjectServer>>();

        public void RegisterComponent(ServiceComponent<ObjectServer> serviceComponent)
        {
            serviceComponent.Register(this);
            _serviceComponents.Add(serviceComponent);
        }

        protected bool CheckExpectations<TMessage>(ClientContext context, TMessage message)
        {
            return _expectations.Check(context, message);
        }

        public TMessage Expect<TMessage>(ClientContext context, Func<TMessage, bool> predicate, int timeout)
        {
            lock (_unhandledMessages)
            {
                foreach (var b in _unhandledMessages)
                {
                    var parts = ObjectString.Split(b);
                    var msg = JsonSerializer.Deserialize<TMessage>(parts[1]);
                }
            }

            return _expectations.Expect<TMessage>(context, predicate, timeout);
        }

        public void RegisterType<TMessage>()
        {
            _callbacks.Add<TMessage>(CheckExpectations<TMessage>);
        }

        public void RegisterType<TMessage>(Func<ClientContext, TMessage, bool> objectReceived)
        {
            _callbacks.Add(objectReceived);
        }

        public void RegisterType<TMessage>(Func<TMessage, bool> objectReceived)
        {
            _callbacks.Add(objectReceived);
        }

        protected override void HandleData(ClientContext context, byte[] data)
        {
            if (DataReceived != null)
            {
                DataReceived(this, Tuple.Create(context, data));
            }

            ThreadPool.QueueUserWorkItem(x =>
            {
                if (!_callbacks.HandleData(context, data))
                {
                    lock (_unhandledMessages)
                    {
                        _unhandledMessages.Add(data);
                    }
                }
            });
        }

        public void WriteObject<TMessage>(ClientContext context, TMessage obj)
        {
            var s = ObjectString.Create(obj);
            context.AcquireWriteLock();

            try
            {
                try
                {
                    context.Writer.WriteData(s);
                }
                catch (Exception e)
                {
                    InvokeClientErrorEncountered(new TcpClientError(context.Client, e));
                }
            }
            finally
            {
                context.ReleaseWriteLock();
            }

            if (DataSent != null)
            {
                DataSent(this, Tuple.Create(context, s.GetBytes()));
            }
        }
    }
}
