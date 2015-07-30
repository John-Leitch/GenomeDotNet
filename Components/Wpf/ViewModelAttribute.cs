using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Wpf
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ViewModelAttribute : Attribute
    {
        public Type ViewModelType { get; private set; }

        public ViewModelAttribute(Type viewModelType)
        {
            ViewModelType = viewModelType;
        }
    }
}
