using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Wpf
{
    public abstract class ItemViewModel<TItem> : ViewModel
    {
        public event ItemEventHandler<PropertyChange<TItem>> ItemChanging, ItemChanged;

        private TItem _item;

        public TItem Item
        {
            get { return _item; }
            set 
            {
                var old = _item;
                OnItemChanging(old, value);
                _item = value;
                InvokePropertyChanged();
                OnItemChanged(old, value);
            }
        }

        public ItemViewModel()
            : base()
        {
        }

        public ItemViewModel(TItem item)
            : this()
        {
            Item = item;
        }

        private void OnItemChanging(TItem oldValue, TItem newValue)
        {
            if (ItemChanging != null)
            {
                ItemChanging(this, new PropertyChange<TItem>(oldValue, newValue));
            }
        }

        private void OnItemChanged(TItem oldValue, TItem newValue)
        {
            if (ItemChanged != null)
            {
                ItemChanged(this, new PropertyChange<TItem>(oldValue, newValue));
            }
        }
    }
}
