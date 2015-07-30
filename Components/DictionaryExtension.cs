using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class DictionaryExtension
    {
        public static void AddOrSet<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary, 
            TKey key, 
            TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }
}
