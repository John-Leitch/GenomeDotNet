using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ObjectClient<TMessage> : ClientBase
    {
        public event ItemEventHandler<TMessage> ObjectReceived;

        public void WriteObject(TMessage message)
        {
            var s = JsonSerializer.Serialize(message);
            Context.AcquireWriteLock();

            try
            {
                Context.Writer.WriteData(s);
            }
            finally
            {
                Context.ReleaseWriteLock();
            }
        }

        protected override void HandleData(ClientContext context, byte[] data)
        {
            var obj = JsonSerializer.Deserialize<TMessage>(data.GetString());

            if (ObjectReceived != null)
            {
                ObjectReceived(this, obj);
            }
        }
    }

    public class ObjectClient : ClientBase
    {
        private TypeCallbackCollection _callbacks = new TypeCallbackCollection();

        private List<ServiceComponent<ObjectClient>> _serviceComponents = new List<ServiceComponent<ObjectClient>>();

        public void RegisterComponent(ServiceComponent<ObjectClient> serviceComponent)
        {
            serviceComponent.Register(this);
            _serviceComponents.Add(serviceComponent);
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
            var parts = ObjectString.Split(data);

            if (parts == null)
            {
                return;
            }

            _callbacks.InvokeDataReceived(new TypeData(context, parts));
        }

        public void WriteObject<TMessage>(TMessage obj)
        {
            var s = ObjectString.Create(obj);
                
            try
            {
                WriteData(s);
            }
            catch (Exception e)
            {
                InvokeClientErrorEncountered(new TcpClientError(Context.Client, e));
            }
        }
    }
}
