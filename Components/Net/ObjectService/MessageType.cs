using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public enum MessageType : byte
    {
        Hello = 0x52,
        Data = 0x53,
        Goodbye = 0x54,
        Error = 0x55,
        NoData = 0xFF
    }
}
