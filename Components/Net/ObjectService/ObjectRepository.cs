using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ObjectRepository<TObject, TPrimaryKey> : JsonRepository<List<TObject>>
        where TPrimaryKey : IEquatable<TPrimaryKey>
    {
        private Func<TObject, TPrimaryKey> _keySelector;

        private string _prefix;

        public ObjectRepository(string prefix, Func<TObject, TPrimaryKey> keySelector)
        {
            _prefix = prefix;
            _keySelector = keySelector;
        }

        private CrossProcessLock CreateLock()
        {
            return new CrossProcessLock(_prefix + "_" + typeof(TObject).FullName + "_repo");
        }

        public void Lock(Action action)
        {
            using (CreateLock())
            {
                action();
            }
        }

        public void Save(TObject obj)
        {
            using (CreateLock())
            {
                var key = _keySelector(obj);
                var list = Load() ?? new List<TObject>();
                var match = list.SingleOrDefault(x => _keySelector(x).Equals(key));

                if (match != null)
                {
                    list.Remove(match);
                }

                list.Add(obj);
                Save(list);
            }
        }

        protected override string GetScriptFile()
        {
            return _prefix + "_" + typeof(TObject).FullName + ".json";
        }
    }
}
