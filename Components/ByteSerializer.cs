using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class BinaryNameValuePair
    {
        public int Order { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public object Value { get; set; }

        public string HexValue
        {
            get { return Value.GetType().IsPrimitive ? string.Format("{0:X8}", Value) : "[none]"; }
        }
    }

    public class BinaryNode
    {
        public IByteSerializable Structure { get; set; }

        public IEnumerable<BinaryNode> Children { get; set; }

        public IEnumerable<BinaryNameValuePair> Fields { get; private set; }

        public BinaryNode()
        {

        }

        public BinaryNode(IByteSerializable structure)
            : this()
        {
            Structure = structure;
            var order = 0;
            Fields = structure
                .GetType()
                .GetProperties()
                .Where<ByteFieldAttribute>()
                .OrderBy(x => x.Attribute.Order)
                .Select(x => new BinaryNameValuePair
                {
                    Order = ++order,
                    Name = x.Property.Name,
                    Type = x.Property.PropertyType.Name.SubstringAtIndexOf('.', 1),
                    Value = x.Property.GetValue(structure),
                })
                .ToArray();

        }

        public BinaryNode(IByteSerializable structure, IEnumerable<BinaryNode> children)
            : this(structure)
        {
            Children = children;
        }
    }

    public static class ByteSerializer
    {
        private static IByteSerializable[] Flatten(List<object> traversed, object obj)
        {
            if (traversed.Contains(obj))
            {
                return new IByteSerializable[0];
            }

            traversed.Add(obj);
            var parts = new List<IByteSerializable>();
            var part = obj as IByteSerializable;

            if (part != null)
            {
                parts.Add(part);
            }

            var children = obj
                .GetType()
                .GetProperties()
                .Where(x => !x.PropertyType.IsPrimitive)
                .Select(x => x.GetValue(obj))
                .Where(x => x != null);

            foreach (var c in children)
            {
                var items = c as IEnumerable<object>;

                if (items == null)
                {
                    parts.AddRange(Flatten(traversed, c));
                }
                else
                {
                    parts.AddRange(
                        items
                            .Where(x => x != null)
                            .SelectMany(x => Flatten(traversed, x)));
                }
            }

            return parts.ToArray();
        }

        private static BinaryNode CreateHierarchy(List<IByteSerializable> traversed, IByteSerializable obj)
        {
            if (traversed.Contains(obj))
            {
                return null;
            }

            traversed.Add(obj);

            var node = new BinaryNode(obj);
            var childNodes = new List<BinaryNode>();

            var children = obj
                .GetType()
                .GetProperties()
                .Where(x => !x.PropertyType.IsPrimitive)
                .Select(x => x.GetValue(obj))
                .Where(x => x != null);
            //.OfType<IByteSerializable>()
            //.ToArray();

            foreach (var c in children)
            {
                IByteSerializable bs = c as IByteSerializable;

                if (bs != null)
                {
                    childNodes.Add(CreateHierarchy(traversed, bs));
                }

                var items = c as IEnumerable<IByteSerializable>;

                if (items != null)
                {
                    childNodes.AddRange(
                        items
                            .Where(x => x != null)
                            .Select(x => CreateHierarchy(traversed, x)));
                }
            }

            node.Children = childNodes;

            return node;
        }

        public static BinaryNode CreateHierarchy(IByteSerializable obj)
        {
            return CreateHierarchy(new List<IByteSerializable>(), obj);
        }

        public static IByteSerializable[] Flatten(object obj)
        {
            return Flatten(new List<object>(), obj);
        }
    }

    public class ByteSerializer<T>
        where T : new()
    {
        public Endianness Endianness { get; set; }

        public ByteSerializer()
        {
            
        }

        public ByteSerializer(Endianness endianness)
            : this()
        {
            Endianness = endianness;
        }

        object InterpretPropertyPath(object obj, string path)
        {
            if (path.Contains('$'))
            {
            }

            var value = obj;

            foreach (var part in path.Split('.'))
            {
                value = value.GetType().GetProperty(part).GetValue(value);
            }

            return value;
        }

        private object Deserialize(T item, BinaryReader reader, Type type)
        {
            var t = typeof(ByteSerializer<>).MakeGenericType(type);
            var childSerializer = Activator.CreateInstance(
                    t,
                    new object[] { Endianness });

            var m = t.GetMethod("Deserialize");

            return m.Invoke(childSerializer, new object[] { reader.BaseStream });
        }

        private object Deserialize(T item, BinaryReader reader, PropertyAttribute<ByteFieldAttribute> ba)
        {

            if (reader.BaseStream.Position >= reader.BaseStream.Length - 1)
            {
                //throw new InvalidOperationException();
                return null;
            }

            IByteSerializable serializable = null;
            var baseOffset = 0;

            if ((serializable = item as IByteSerializable) != null)
            {
                baseOffset = serializable.Index;
            }

            if (ba.Attribute.OffsetPath != null)
            {
                var offset = Convert.ToInt32(InterpretPropertyPath(item, ba.Attribute.OffsetPath));

                if (offset < 0)
                {
                    return null;
                }

                if (ba.Attribute.IsRelative)
                {
                    if (serializable == null)
                    {
                        throw new InvalidOperationException();
                    }

                    offset += serializable.Index;
                }

                reader.BaseStream.Position = offset;
            }

            var p = ba.Property.PropertyType;

            object o =
                p == typeof(decimal) ? reader.ReadDecimal() :
                p == typeof(double) ? reader.ReadDouble() :
                p == typeof(short) ? reader.ReadInt16() :
                p == typeof(int) ? reader.ReadInt32() :
                p == typeof(long) ? reader.ReadInt64() :
                p == typeof(sbyte) ? reader.ReadSByte() :
                p == typeof(float) ? reader.ReadSingle() :
                p == typeof(string) ? reader.ReadString() :
                p == typeof(ushort) ? reader.ReadUInt16() :
                p == typeof(uint) ? reader.ReadUInt32() :
                p == typeof(ulong) ? reader.ReadUInt64() :
                default(object);

            if (o == null)
            {
                if (!ba.Property.PropertyType.IsArray)
                {
                    o = Deserialize(item, reader, ba.Property.PropertyType);
                }
                else
                {
                    if (ba.Attribute.LengthPath == null)
                    {
                        throw new InvalidOperationException();
                    }

                    var lengthStr = InterpretPropertyPath(item, ba.Attribute.LengthPath);
                    var length = Convert.ToUInt32(lengthStr);
                    var et = ba.Property.PropertyType.GetElementType();

                    if (p == typeof(ushort[]))
                    {
                        var ushorts = new ushort[length];
                        var childSerialzer = new ByteSerializer<ushort>(Endianness);
                        for (int i = 0; i < length; i++)
                        {
                            ushorts[i] = childSerialzer.Deserialize(reader.BaseStream);
                        }

                        o = ushorts;
                    }
                    else
                    {
                        var a = Array.CreateInstance(et, length);

                        for (int i = 0; i < length; i++)
                        {
                            a.SetValue(Deserialize(item, reader, et), i);
                        }

                        o = a;

                    }

                }
            }

            //if (ba.Attribute.Store)
            //{
            //    Store.Add(ba.Property.Name, o);
            //}

            return o;
        }

        public T Deserialize(Stream stream)
        {
            //Console.WriteLine("Deserializing {0}", typeof(T).Name);
            if (stream.Position >= stream.Length - 1)
            {
                return default(T);
            }

            BinaryReader reader = Endianness == Endianness.Little ?
                new BinaryReader(stream) : new BigEndianBinaryReader(stream);

            var byteAttribute = typeof(T)
                .GetProperties()
                .Where<ByteFieldAttribute>()
                .OrderBy(x => x.Attribute.Order);

            T item;

            if (!typeof(T).IsPrimitive)
            {
                item = new T();

                IByteSerializable serializable;

                if ((serializable = item as IByteSerializable) != null)
                {
                    serializable.Index = (int)stream.Position;
                    serializable.OnDeserializing(stream);
                }
                else
                {
                    throw new InvalidOperationException();
                }

                foreach (var ba in byteAttribute)
                {
                    if (serializable != null)
                    {
                        serializable.OnDeserializingField(stream, ba.Property, ba.Attribute);
                    }                    

                    var o = Deserialize(item, reader, ba);
                    ba.Property.SetValue(item, o);

                    if (serializable != null)
                    {
                        serializable.OnDeserializedField(stream, ba.Property, ba.Attribute);
                    }                    
                }

                if (serializable != null)
                {
                    serializable.OnDeserialized(stream);
                    serializable.StructureLength = (int)stream.Position - serializable.Index;

                    stream.Position = serializable.Index;
                    serializable.Raw = stream.Read(serializable.StructureLength);

                    //if (serializable.Index <= 119119 &&
                    //    119119 <= serializable.Index + serializable.StructureLength)
                    //{
                    //    throw new InvalidOperationException();
                    //}
                }
            }
            else
            {
                var index = 0;
                try
                {
                    index = (int)reader.BaseStream.Position;

                    if (typeof(T) == typeof(decimal)) { return (T)(object)reader.ReadDecimal(); }
                    else if (typeof(T) == typeof(double)) { return (T)(object)reader.ReadDouble(); }
                    else if (typeof(T) == typeof(short)) { return (T)(object)reader.ReadInt16(); }
                    else if (typeof(T) == typeof(int)) { return (T)(object)reader.ReadInt32(); }
                    else if (typeof(T) == typeof(long)) { return (T)(object)reader.ReadInt64(); }
                    else if (typeof(T) == typeof(sbyte)) { return (T)(object)reader.ReadSByte(); }
                    else if (typeof(T) == typeof(float)) { return (T)(object)reader.ReadSingle(); }
                    else if (typeof(T) == typeof(string)) { return (T)(object)reader.ReadString(); }
                    else if (typeof(T) == typeof(byte)) { return (T)(object)reader.ReadByte(); }
                    else if (typeof(T) == typeof(ushort)) { return (T)(object)reader.ReadUInt16(); }
                    else if (typeof(T) == typeof(uint)) { return (T)(object)reader.ReadUInt32(); }
                    else if (typeof(T) == typeof(ulong)) { return (T)(object)reader.ReadUInt64(); }
                }
                finally
                {
                    //if (index <= 119119 && 119119 <= reader.BaseStream.Position)
                    //{
                    //    throw new InvalidOperationException();
                    //}
                    //var len = (int)reader.BaseStream.Position - index;
                    //Console.WriteLine();
                }
                throw new InvalidOperationException();
            }



            return item;
        }

        public T[] DeserializeArray(Stream stream, int count)
        {
            var a = new T[count];

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = Deserialize(stream);
            }

            return a;
        }

        public T[] DeserializeArray(Stream stream, uint count)
        {
            var a = new T[count];

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = Deserialize(stream);
            }

            return a;
        }

        
    }
}
