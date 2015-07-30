using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class TypeCallbackCollection
    {
        private event ItemEventHandler<TypeData> DataReceived;

        private List<TypeData> _unhandledData = new List<TypeData>();

        public void InvokeDataReceived(TypeData data)
        {
            if (DataReceived != null)
            {
                DataReceived(this, data);
            }
        }

        private List<Type> _registeredTypes = new List<Type>();

        public void Add<TMessage>(Func<TMessage, bool> objectReceived)
        {
            Add((ClientContext c, TMessage m) => objectReceived(m));
        }

        public void Add<TMessage>(Func<ClientContext, TMessage, bool> objectReceived)
        {
            if (_registeredTypes.Contains(typeof(TMessage)))
            {
                throw new InvalidOperationException();
            }

            _registeredTypes.Add(typeof(TMessage));
            var name = typeof(TMessage).FullName;

            ItemEventHandler<TypeData> handler = (o, e) =>
            {
                if (e.Item.IsHandled || e.Item.Parts[0] != name)
                {
                    return;
                }

                e.Item.IsHandled = objectReceived(
                    e.Item.Context, 
                    JsonSerializer.Deserialize<TMessage>(e.Item.Parts[1]));
            };

            lock (_unhandledData)
            {
                foreach (var unhandled in _unhandledData.ToArray())
                {
                    handler(this, unhandled);

                    if (unhandled.IsHandled)
                    {
                        _unhandledData.Remove(unhandled);
                    }
                }
            }

            DataReceived += handler;
        }

        public bool HandleData(ClientContext context, byte[] data)
        {
            var parts = ObjectString.Split(data);

            if (parts == null)
            {
                throw new InvalidOperationException();
            }

            var data2 = new TypeData(context, parts);
            InvokeDataReceived(data2);
            
            if (!data2.IsHandled)
            {
                lock (_unhandledData)
                {
                    _unhandledData.Add(data2);
                }
            }

            return data2.IsHandled;
        }
    }
}
