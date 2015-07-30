using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Components.Wpf
{
    public abstract class CompositeControl : UserControl, ICompositeControl
    {
        public int Order { get; set; }

        public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register(
                "HeaderContent",
                typeof(object),
                typeof(CompositeControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentHeaderChanged)));

        public object HeaderContent
        {
            get { return (object)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        public static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public static void OnContentHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
