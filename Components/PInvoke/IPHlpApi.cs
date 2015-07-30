using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Components.PInvoke
{
    public static class IPHlpApi
    {
        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern uint GetExtendedTcpTable(
            IntPtr pTcpTable,
            ref int dwOutBufLen,
            bool sort,
            _AddressFamily ipVersion,
            TCP_TABLE_CLASS tblClass,
            uint reserved = 0);
    }
}
