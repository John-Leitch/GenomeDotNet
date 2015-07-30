using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public abstract class ByteSerializable : IByteSerializable
    {
        public int Index { get; set; }

        public int StructureLength { get; set; }

        public byte[] Raw { get; set; }

        public virtual void OnDeserializing(System.IO.Stream stream)
        {

        }

        public virtual void OnDeserializingField(System.IO.Stream stream, PropertyInfo property, ByteFieldAttribute field)
        {

        }

        public virtual void OnDeserializedField(System.IO.Stream stream, PropertyInfo property, ByteFieldAttribute field)
        {

        }

        public virtual void OnDeserialized(System.IO.Stream stream)
        {

        }
    }
}
