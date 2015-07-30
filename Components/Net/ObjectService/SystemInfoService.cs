using Components;
using Components.Net;
using Components.Net.ObjectService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class SystemInfoService : BackgroundService
    {
        public ItemEventHandler<Exception> WriteFailed;

        private ObjectClient _client;

        public SystemInfoService(ObjectClient client)
        {
            Interval = 10000;
            //Interval = 9999999;
            _client = client;
        }

        protected override void ServiceAction()
        {
            if (!_client.Context.Client.Connected)
            {
                Stop(false);
            }

            var o = SystemInfo.Create();

            try
            {
                _client.WriteObject(o);
            }
            catch (Exception e)
            {
                if (WriteFailed != null)
                {
                    WriteFailed(this, e);
                }

                Debug.WriteLine("Write failed: {0}", e);
            }
        }
    }
}
