using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public delegate void ItemEventHandler<TItem>(object sender, ItemEventArgs<TItem> e);
}
