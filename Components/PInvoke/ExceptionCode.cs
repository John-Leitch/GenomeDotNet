﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.PInvoke
{
    public static class ExceptionCode
    {
        public const uint EXCEPTION_ACCESS_VIOLATION = 0xc0000005,
            EXCEPTION_BREAKPOINT = 0x80000003;
    }
}
