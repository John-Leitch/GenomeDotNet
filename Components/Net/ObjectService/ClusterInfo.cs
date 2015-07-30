using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ClusterInfo
    {
        public ObservableCollection<string> KnownSystems { get; set; }

        public ObservableCollection<SystemGroup> Groups { get; set; }

        public ClusterInfo()
        {
            KnownSystems = new ObservableCollection<string>();
            Groups = new ObservableCollection<SystemGroup>();
        }

        public void Add(SystemInfo systemInfo)
        {
            lock (KnownSystems)
            {
                if (!KnownSystems.Contains(systemInfo.MachineName))
                {
                    KnownSystems.Add(systemInfo.MachineName);
                }
            }
        }
    }
}
