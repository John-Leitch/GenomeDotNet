using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Components.PInvoke
{
    public static class MSVCRT
    {
        [DllImport("msvcrt.dll", SetLastError = true)]
        public static extern int system(string command);
    }
}
