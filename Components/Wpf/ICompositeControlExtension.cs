using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Components.Wpf
{
    public static class ICompositeControlExtension
    {
        public static void CreateViewModel(this ICompositeControl c)
        {
            var contentElement = c as FrameworkElement;

            if (contentElement == null)
            {
                return;
            }

            var vmAttribute = c
                .GetType()
                .GetCustomAttributes(
                    typeof(ViewModelAttribute),
                    true)
                .Cast<ViewModelAttribute>()
                .SingleOrDefault() as ViewModelAttribute;

            if (vmAttribute == null)
            {
                return;
            }
            
            var vm = (ViewModel)Activator.CreateInstance(
                vmAttribute.ViewModelType);

            vm.Initialize();
            contentElement.DataContext = vm;
        }
    }
}
