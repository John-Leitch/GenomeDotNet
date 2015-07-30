using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EXCEPTION_POINTERS
    {
        public IntPtr ExceptionRecord;
        public IntPtr ContextRecord;
    }
}
