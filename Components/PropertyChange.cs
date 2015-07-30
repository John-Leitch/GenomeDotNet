using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class PropertyChange<TProperty>
    {
        public TProperty OldValue { get; private set; }

        public TProperty NewValue { get; private set; }

        public PropertyChange(TProperty oldValue, TProperty newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
