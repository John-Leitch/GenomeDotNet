using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Components.Wpf
{
    public abstract class ViewModel : Bindable
    {
        public virtual void Initialize()
        {
        }

        public static TViewModel Get<TViewModel>(object obj)
            where TViewModel : ViewModel
        {
            return ((FrameworkElement)obj).DataContext as TViewModel;
        }

        [DebuggerStepThrough]
        [DebuggerNonUserCode]
        public static void Action<TViewModel>(object obj, Action<TViewModel> action)
            where TViewModel : ViewModel
        {
            var vm = Get<TViewModel>(obj);

            if (vm != null)
            {
                action(vm);
            }
            else if (Debugger.IsAttached)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
