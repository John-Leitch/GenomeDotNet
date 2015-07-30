using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Components.Net.ObjectService
{
    public sealed class Expectation<TMessage> : IDisposable
    {
        public ClientContext Context { get; private set; }

        public Func<TMessage, bool> Predicate { get; private set; }

        public Action<TMessage> Callback { get; private set; }

        public ManualResetEvent Reset { get; private set; }

        public Expectation(ClientContext context, Func<TMessage, bool> predicate, Action<TMessage> callback)
        {
            Context = context;
            Reset = new ManualResetEvent(false);
            Predicate = predicate;
            Callback = callback;
        }

        public void Dispose()
        {
            if (Reset != null)
            {
                Reset.Dispose();
            }
        }

        public bool Check(ClientContext context, TMessage message)
        {
            if ((Context == null || Context == context) && Predicate(message))
            {
                Callback(message);
                Reset.Set();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
