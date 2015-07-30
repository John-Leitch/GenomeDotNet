using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public interface IByteSerializable
    {
        int Index { get; set; }

        int StructureLength { get; set; }

        byte[] Raw { get; set; }

        void OnDeserializing(Stream stream);

        void OnDeserializingField(Stream stream, PropertyInfo property, ByteFieldAttribute field);

        void OnDeserializedField(Stream stream, PropertyInfo property, ByteFieldAttribute field);

        void OnDeserialized(Stream stream);
    }
}
