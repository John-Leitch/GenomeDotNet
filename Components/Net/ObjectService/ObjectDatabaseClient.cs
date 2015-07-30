using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ObjectDatabaseClient<TObject, TPrimaryKey>
        where TPrimaryKey : IEquatable<TPrimaryKey>
    {
        private ObjectRepository<TObject, TPrimaryKey> _repository;

        private ObjectClient _client;

        private BackgroundAction _thread;

        public ObjectDatabaseClient(ObjectClient client, Func<TObject, TPrimaryKey> keySelector)
        {
            _client = client;
            _repository = new ObjectRepository<TObject, TPrimaryKey>("Client", keySelector);
            _thread = new BackgroundAction(SaveRepo) { Interval = 1000 };
            _thread.StartAsync();
        }

        public void Save(TObject obj)
        {
            _client.WriteObject(obj);
            //_repository.Save(obj, _keySelector);
        }

        private void SaveRepo()
        {
            _repository.Lock(() =>
            {
                var repo = _repository.Load() ?? new List<TObject>();

                if (!repo.Any())
                {
                    return;
                }

                foreach (var r in repo.ToArray())
                {
                    Save(r);
                    repo.Remove(r);
                }

                _repository.Save(repo);
            });
        }
    }
}
