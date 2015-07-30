using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Components.Wpf
{
    public static class WebBrowserExtensions
    {
        public static void SetSilent(this WebBrowser webBrowser, bool silent)
        {
            var axBrowserField = typeof(WebBrowser).GetField(
                "_axIWebBrowser2",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (axBrowserField == null)
            {
                throw new InvalidOperationException();
            }

            object comBowser = null;

            int tries = 0;

            while (true)
            {
                if (tries++ > 100)
                {
                    throw new InvalidOperationException();
                }

                comBowser = axBrowserField.GetValue(webBrowser);

                if (comBowser == null)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    break;
                }
            }

            comBowser.GetType().InvokeMember(
                "Silent",
                BindingFlags.SetProperty,
                null,
                comBowser,
                new object[] { silent });
        }
    }
}
