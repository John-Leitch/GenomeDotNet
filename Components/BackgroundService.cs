using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Components
{
    public abstract class BackgroundService
    {
        private object _startSync = new object();

        private Thread _thread;

        public bool IsStarted { get; private set; }

        public int Interval { get; set; }

        public BackgroundService()
        {
            Interval = 10000;
        }

        protected abstract void ServiceAction();

        public void StartAsync()
        {
            lock (_startSync)
            {
                if (IsStarted)
                {
                    throw new InvalidOperationException();
                }

                IsStarted = true;
                _thread = new Thread(() =>
                {
                    while (IsStarted)
                    {
                        ServiceAction();

                        Thread.Sleep(Interval);
                    }
                });

                _thread.Start();
            }
        }

        public void Stop(bool join)
        {
            lock (_startSync)
            {
                if (!IsStarted)
                {
                    throw new InvalidOperationException();
                }

                IsStarted = false;

                if (join)
                {
                    _thread.Join();
                }

                _thread = null;
            }
        }

        public void Stop()
        {
            Stop(true);
        }
    }
}
