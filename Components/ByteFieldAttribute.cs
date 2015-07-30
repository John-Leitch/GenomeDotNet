using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class ByteFieldAttribute : Attribute
    {
        public int Order { get; private set; }

        public string LengthPath { get; set; }

        public string OffsetPath { get; set; }

        public bool IsRelative { get; set; }

        public bool Store { get; set; }

        //public Type Type { get; private set; }

        public ByteFieldAttribute(int order)
        {
            Order = order;
            //Type = type;
        }

        //public ByteFieldAttribute(int order, string name)
        //    : this(order)
        //{
        //    LengthPath = name;
        //}
    }

    public class ByteArrayFieldAttribute : ByteFieldAttribute
    {
        public Func<object, int> CountSelector { get; private set; }

        public ByteArrayFieldAttribute(int order, Func<object, int> countSelector)
            : base(order)
        {
            CountSelector = countSelector;
        }
    }
}
