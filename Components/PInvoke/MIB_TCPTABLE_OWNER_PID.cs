using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Components.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPTABLE_OWNER_PID
    {
        public uint dwNumEntries;
        MIB_TCPROW_OWNER_PID table;
    }
}
