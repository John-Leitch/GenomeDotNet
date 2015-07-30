using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Components
{
    public static class ThreadExtension
    {
        public static bool TryKill(this Thread thread)
        {
            try
            {
                thread.Abort();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
