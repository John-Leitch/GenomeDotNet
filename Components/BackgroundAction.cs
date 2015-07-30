using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class BackgroundAction : BackgroundService
    {
        private Action _action;

        public BackgroundAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentException();
            }

            _action = action;
        }

        protected override void ServiceAction()
        {
            _action();
        }

        public static BackgroundAction StartAsync(int interval, Action action)
        {
            var a = new BackgroundAction(action) { Interval = interval };
            a.StartAsync();
            return a;
        }
    }
}
