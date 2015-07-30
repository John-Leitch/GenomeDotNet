using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public abstract class ServiceComponent<TNetworkComponent>
    {
        public bool IsRegistered { get; protected set; }

        public TNetworkComponent Network { get; protected set; }

        protected abstract void RegisterCore(TNetworkComponent network);

        public void Register(TNetworkComponent network)
        {
            if (IsRegistered)
            {
                throw new InvalidOperationException();
            }

            RegisterCore(network);
            IsRegistered = true;
            Network = network;
        }
    }
}
