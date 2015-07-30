using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class ClassLoader
    {
        public List<Assembly> Assemblies { get; set; }

        public List<string> Suffixes { get; set; }

        public ClassLoader()
        {
            Assemblies = new List<Assembly>();
            Suffixes = new List<string>();
        }

        public Type LoadType(string className)
        {
            IEnumerable<string> names = new[] { className };

            if (Suffixes != null && Suffixes.Any())
            {
                names = names.Concat(Suffixes.Select(x => className + x));
            }

            var types = Assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => names.Contains(x.Name))
                .ToArray();

            if (types.Count() > 1)
            {
                throw new InvalidOperationException();
            }

            var type = types.SingleOrDefault();

            if (type == null)
            {
                return null;
            }

            return type;
        }

        public MethodInfo LoadMethod(string fullMethodName)
        {
            var nameIndex = fullMethodName.LastIndexOf('.');
            var className = fullMethodName.Remove(nameIndex);
            var methodName = fullMethodName.Substring(nameIndex + 1);
            var type = LoadType(className);
            var method = type.GetMethod(methodName);
            return method;
        }

        public TDelegate LoadMethodDelegate<TDelegate>(string fullMethodName)
        {
            var method = LoadMethod(fullMethodName);
            var methodDelegate = (TDelegate)(object)method.CreateDelegate(typeof(TDelegate));
            return methodDelegate;
        }

        public T Load<T>(string className)
            where T : class
        {
            var t = LoadType(className);
            return (T)Activator.CreateInstance(t);
        }
    }
}
