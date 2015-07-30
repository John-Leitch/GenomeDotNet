using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace Components.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPROW_OWNER_PID
    {
        public TcpState state;
        public uint localAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] localPort;
        public uint remoteAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] remotePort;
        public uint owningPid;

        public IPAddress GetLocalAddress()
        {
            return new IPAddress(localAddr);
        }

        public ushort GetLocalPort()
        {
            return BitConverter.ToUInt16(
                new byte[2] { localPort[1], localPort[0] }, 
                0);
        }

        public IPAddress GetRemoteAddress()
        {
            return new IPAddress(remoteAddr);
        }

        public ushort GetRemotePort()
        {
            return BitConverter.ToUInt16(
                new byte[2] { remotePort[1], remotePort[0] }, 
                0);
        }
    }
}
