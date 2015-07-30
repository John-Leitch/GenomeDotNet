using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Components.Wpf
{
    public static class FrameworkElementExtension
    {
        public static TViewModel Get<TViewModel>(this FrameworkElement element)
            where TViewModel : ViewModel
        {
            return ViewModel.Get<TViewModel>(element);
        }

        public static void Action<TViewModel>(this FrameworkElement element, Action<TViewModel> action)
            where TViewModel : ViewModel
        {
            ViewModel.Action<TViewModel>(element, action);
        }
    }
}
