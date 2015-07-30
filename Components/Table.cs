using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class Table<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _table = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get
            {
                TValue a;

                if (!_table.TryGetValue(key, out a))
                {
                    return default(TValue);
                }

                return a;
            }
            set
            {
                if (!_table.ContainsKey(key))
                {
                    _table.Add(key, value);
                }
                else
                {
                    _table[key] = value;
                }
            }
        }
    }
}
