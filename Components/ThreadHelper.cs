using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Components
{
    public static class ThreadHelper
    {
        public static Thread DoWhile(Func<bool> condition, Action action, int sleepTime, ManualResetEvent finishReset)
        {
            var t = new Thread(() =>
            {
                while (condition())
                {
                    action();
                    Thread.Sleep(sleepTime);
                }

                if (finishReset != null)
                {
                    finishReset.Set();
                }
            }) { IsBackground = true };

            t.Start();

            return t;
        }

        public static Thread DoWhile(Func<bool> condition, Action action, int sleepTime)
        {
            return DoWhile(condition, action, sleepTime, null);
        }

        public static Thread DoBackground(ThreadStart action)
        {
            var t = new Thread(action) { IsBackground = true };
            t.Start();

            return t;
        }
    }
}
