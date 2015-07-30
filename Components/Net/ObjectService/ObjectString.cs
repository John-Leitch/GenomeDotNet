using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public static class ObjectString
    {
        public static string[] Split(byte[] data)
        {
            var dataString = data.GetString();
            var index = dataString.IndexOf(':');

            if (index == -1)
            {
                return null;
            }

            var parts = dataString.Split(index);
            parts[1] = parts[1].Substring(1);

            return parts;
        }

        public static string Create<TMessage>(TMessage message)
        {
            return typeof(TMessage).FullName + ":" + JsonSerializer.Serialize(message);
        }
    }
}
