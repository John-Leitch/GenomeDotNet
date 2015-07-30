using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Components.Wpf
{
    public class ControlLoader : TypeLoader
    {
        public IEnumerable<ICompositeControl> LoadControls()
        {
            return LoadType<ICompositeControl>().OrderBy(x => x.Order);
        }
    }
}
