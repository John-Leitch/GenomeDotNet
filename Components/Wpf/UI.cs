using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Components.Wpf
{
    public static class UI
    {
        public static void Invoke(Action action)
        {
            if (Application.Current != null)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(action);
                }
                catch (TaskCanceledException e)
                {
                    Debug.Print(e.ToString());
                }
            }
            else
            {
                action();
            }
        }

        public static void BeginInvoke(Action action)
        {
            if (Application.Current != null)
            {
                try
                {
                    Application.Current.Dispatcher.BeginInvoke(action);
                }
                catch (TaskCanceledException e)
                {
                    Debug.Print(e.ToString());
                }
            }
            else
            {
                action();
            }
        }

        public static void Invoke<T>(Action<T> action, T arg)
        {
            if (Application.Current != null)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => action(arg));
                }
                catch (TaskCanceledException e)
                {
                    Debug.Print(e.ToString());
                }
            }
            else
            {
                action(arg);
            }
        }

        public static void BeginInvoke<T>(Action<T> action, T arg)
        {
            if (Application.Current != null)
            {
                try
                {
                    Application.Current.Dispatcher.BeginInvoke((Action)(() => action(arg)));
                }
                catch (TaskCanceledException e)
                {
                    Debug.Print(e.ToString());
                }
            }
            else
            {
                action(arg);
            }
        }
    }
}
