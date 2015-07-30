using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public abstract class JsonRepository<TEntities> : IRepository<TEntities>
    {
        protected abstract string GetScriptFile();

        private bool ScriptFileExists()
        {
            return File.Exists(GetScriptFile());
        }

        public TEntities Load()
        {
            if (!ScriptFileExists())
            {
                return default(TEntities);
            }

            return JsonSerializer.DeserializeFile<TEntities>(GetScriptFile());
        }

        public void Save(TEntities entities)
        {
            JsonSerializer.SerializeToFile(GetScriptFile(), entities);
        }
    }
}
