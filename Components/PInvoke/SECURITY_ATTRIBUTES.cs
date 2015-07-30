using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public unsafe byte* lpSecurityDescriptor;
        public int bInheritHandle;
    }
}
