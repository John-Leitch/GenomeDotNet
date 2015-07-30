using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class PropertyInfoExtension
    {
        public static PropertyAttribute<TAttribute> FirstByAttribute<TAttribute>(
            this IEnumerable<PropertyInfo> properties)
            where TAttribute : Attribute
        {
            foreach (var p in properties)
            {
                var a = p.GetCustomAttribute<TAttribute>();

                if (a != null)
                {
                    return new PropertyAttribute<TAttribute>(p, a);
                }
            }

            throw new InvalidOperationException();
        }

        public static IEnumerable<PropertyAttribute<TAttribute>> Where<TAttribute>(
            this IEnumerable<PropertyInfo> properties)
            where TAttribute : Attribute
        {
            var pas = new List<PropertyAttribute<TAttribute>>();

            foreach (var p in properties)
            {
                var a = p.GetCustomAttribute<TAttribute>();

                if (a != null)
                {
                    pas.Add(new PropertyAttribute<TAttribute>(p, a));
                }
            }

            return pas;
        }
    }
}
