using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class ObjectDatabaseServerComponent<TObject, TPrimaryKey> : ServiceComponent<ObjectServer>
        where TPrimaryKey : IEquatable<TPrimaryKey>
    {
        private ObjectRepository<TObject, TPrimaryKey> _repository;

        public ObjectDatabaseServerComponent(Func<TObject, TPrimaryKey> keySelector)
        {
            _repository = new ObjectRepository<TObject, TPrimaryKey>("Server", keySelector);
        }

        protected override void RegisterCore(ObjectServer network)
        {
            network.RegisterType<TObject>(ObjectReceived);
        }

        private bool ObjectReceived(TObject obj)
        {
            _repository.Save(obj);

            return true;
        }
    }
}
