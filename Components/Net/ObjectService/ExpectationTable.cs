using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ExpectationTable
    {
        private Dictionary<Type, IEnumerable> _collectionTable = new Dictionary<Type, IEnumerable>();

        public List<Expectation<TElement>> GetList<TElement>()
        {
            lock (_collectionTable)
            {
                if (!_collectionTable.ContainsKey(typeof(TElement)))
                {
                    _collectionTable.Add(typeof(TElement), new List<Expectation<TElement>>());
                }

                return (List<Expectation<TElement>>)_collectionTable[typeof(TElement)];
            }
        }

        private void Add<TElement>(Expectation<TElement> element)
        {
            if (!_collectionTable.ContainsKey(typeof(TElement)))
            {
                _collectionTable.Add(typeof(TElement), new List<Expectation<TElement>> { element });
            }
            else
            {
                GetList<TElement>().Add(element);
            }
        }

        private void Remove<TElement>(Expectation<TElement> element)
        {
            if (_collectionTable.ContainsKey(typeof(TElement)))
            {
                GetList<TElement>().Remove(element);
            }
        }

        public TMessage Expect<TMessage>(ClientContext context, Func<TMessage, bool> predicate, int timeout)
        {
            TMessage message = default(TMessage);

            using (var expectation = new Expectation<TMessage>(context, predicate, x => message = x))
            {
                try
                {
                    Add(expectation);

                    if (!expectation.Reset.WaitOne(timeout))
                    {
                        throw new InvalidOperationException();
                    }
                }
                finally
                {
                    lock (this)
                    {
                        Remove(expectation);
                    }
                }
            }

            return message;
        }

        public bool Check<TMessage>(ClientContext context, TMessage message)
        {
            lock (_collectionTable)
            {
                foreach (var ex in GetList<TMessage>())
                {
                    if (ex.Check(context, message))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
