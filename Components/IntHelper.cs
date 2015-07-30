using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class IntHelper
    {
        public static int Parse(string s)
        {
            if (s.StartsWith("0x"))
            {
                return Convert.ToInt32(s.Substring(2), 16);
            }
            else
            {
                return int.Parse(s);
            }
        }
    }
}
