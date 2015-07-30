using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class PropertyAttribute<TAttribute>
        where TAttribute : Attribute
    {
        public PropertyInfo Property { get; private set; }

        public TAttribute Attribute { get; private set; }

        public PropertyAttribute(PropertyInfo property, TAttribute attribute)
        {
            Property = property;
            Attribute = attribute;
        }
    }
}
