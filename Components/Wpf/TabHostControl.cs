using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Components.Wpf
{
    public class TabHostControl : HostControl
    {
        private TabControl _tabs = new TabControl();

        private Dictionary<ICompositeControl, TabItem> _tabTable = new Dictionary<ICompositeControl, TabItem>();

        public TabHostControl()
        {
            Content = _tabs;
        }

        protected override void OnItemAdded(ICompositeControl item)
        {
            var tab = new TabItem()
            {
                Header = item.HeaderContent,
                Content = item,
            };

            _tabs.Items.Add(tab);
            item.CreateViewModel();
            _tabTable.Add(item, tab);
        }

        protected override void OnItemRemoved(ICompositeControl item)
        {
            _tabs.Items.Remove(_tabTable[item]);
        }
    }
}
