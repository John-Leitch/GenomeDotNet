using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class TypeLoader
    {
        private List<Assembly> _assemblies = new List<Assembly>();

        public List<Assembly> Assemblies
        {
            get { return _assemblies; }
        }

        public static IEnumerable<TPlugin> LoadType<TPlugin>(Assembly asm)
        {
            return asm
                .GetTypes()
                .Where(x => x.IsDerivedFromOrImplements<TPlugin>())
                .Select(Activator.CreateInstance)
                .Cast<TPlugin>();
        }

        public IEnumerable<TPlugin> LoadType<TPlugin>()
        {
            return _assemblies.SelectMany(LoadType<TPlugin>);
        }
    }
}
