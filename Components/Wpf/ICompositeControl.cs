using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Wpf
{
    public interface ICompositeControl
    {
        int Order { get; set; }

        object HeaderContent { get; set; }

        object Content { get; set; }
    }
}
