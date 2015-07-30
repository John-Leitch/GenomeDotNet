﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Components;

namespace Components
{
    [DebuggerStepThrough]
    public static class StreamExtension
    {
        public static byte[] Read(this Stream sourceStream, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];

            int len = sourceStream.Read(buffer, 0, bufferSize);

            Array.Resize(ref buffer, len);

            return buffer;
        }

        public static byte[] ReadAll(this Stream sourceStream, int length)
        {
            var bytes = new List<byte>();

            while (bytes.Count < length)
            {
                bytes.AddRange(sourceStream.Read(length - bytes.Count));
            }

            return bytes.ToArray();
        }

        public static void Write(this Stream destinationStream, byte[] buffer)
        {
            destinationStream.Write(buffer, 0, buffer.Length);
        }

        public static void Write(this Stream sourceStream, Stream destinationStream)
        {
            byte[] bytes;

            while ((bytes = sourceStream.Read(8192)).Length != 0)
                destinationStream.Write(bytes);
        }

        public static string ReadString(this Stream destinationStream, int bufferSize)
        {
            return Read(destinationStream, bufferSize).GetString();
        }

        public static void WriteString(this Stream destinationStream, string buffer)
        {
            Write(destinationStream, buffer.GetBytes());
        }
    }
}
