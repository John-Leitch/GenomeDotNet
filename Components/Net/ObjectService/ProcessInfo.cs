using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Components.Net.ObjectService
{
    public class ProcessInfo
    {
        public StringBuilder Output { get; set; }

        public Process Process { get; set; }

        public ProcessInfo()
        {
            Output = new StringBuilder();
        }
    }
}
