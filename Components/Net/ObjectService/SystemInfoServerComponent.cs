using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components.Wpf;

namespace Components.Net.ObjectService
{
    public class SystemInfoServerComponent : ServiceComponent<ObjectServer>
    {
        public ObservableCollection<SystemInfo> Systems { get; private set; }

        public event ItemEventHandler<SystemInfo> SystemInfoReceived;

        private Dictionary<string, ClientContext> _machineTable;

        public SystemInfoServerComponent()
        {
            Systems = new ObservableCollection<SystemInfo>();
            _machineTable = new Dictionary<string, ClientContext>();
        }

        protected override void RegisterCore(ObjectServer network)
        {
            network.RegisterType<SystemInfo>(SystemInfoReceivedCallback);
        }

        private bool SystemInfoReceivedCallback(ClientContext context, SystemInfo info)
        {
            info.DateTime = DateTime.Now;

            if (SystemInfoReceived != null)
            {
                SystemInfoReceived(this, info);
            }

            lock (_machineTable)
            {
                if (_machineTable.ContainsKey(info.MachineName))
                {
                    _machineTable[info.MachineName] = context;
                }
                else
                {
                    _machineTable.Add(info.MachineName, context);
                }
            }

            lock (Systems)
            {
                var existing = Systems.SingleOrDefault(x => x.MachineName == info.MachineName);

                if (existing != null)
                {
                    UI.Invoke(() =>
                    {
                        existing.DateTime = info.DateTime;
                        existing.CpuUsage = info.CpuUsage;
                        existing.MemoryUsage = info.MemoryUsage;
                    });
                }
                else
                {
                    UI.Invoke(() => Systems.Add(info));
                }
            }

            return true;
        }

        public ClientContext GetContext(string machineName)
        {
            ClientContext c;

            lock (machineName)
            {
                return !_machineTable.TryGetValue(machineName, out c) ? null : c;
            }
        }
    }
}
