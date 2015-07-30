using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Components.Wpf
{
    public abstract class HostControl : CompositeControl
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable<ICompositeControl>),
                typeof(HostControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ItemsSourceChanged)));

        public IEnumerable<ICompositeControl> ItemsSource
        {
            get { return (IEnumerable<ICompositeControl>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var host = (HostControl)d;
            host.SubscribeAndUnsubscribeCollectionChanged(e);

            foreach (var i in host.ItemsSource)
            {
                host.OnItemAdded(i);
            }
        }

        private void CollectionAction(object value, Action<INotifyCollectionChanged> action)
        {
            var collection = value as INotifyCollectionChanged;

            if (collection == null)
            {
                return;
            }

            action(collection);            
        }

        private void SubscribeAndUnsubscribeCollectionChanged(DependencyPropertyChangedEventArgs e)
        {
            CollectionAction(e.OldValue, x => x.CollectionChanged -= CollectionChanged);
            CollectionAction(e.NewValue, x => x.CollectionChanged += CollectionChanged);
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var i in e.NewItems.Cast<ICompositeControl>())
            {
                OnItemAdded(i);
            }

            foreach (var i in e.OldItems.Cast<ICompositeControl>())
            {
                OnItemRemoved(i);
            }
        }

        protected abstract void OnItemAdded(ICompositeControl item);

        protected abstract void OnItemRemoved(ICompositeControl item);
    }
}
