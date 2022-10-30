using Inventory.Uwp.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views
{
    public sealed partial class TwoPaneViewDetailControl : UserControl
    {
        public SampleOrder SelectedItem
        {
            get { return GetValue(SelectedItemProperty) as SampleOrder; }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(SampleOrder), typeof(TwoPaneViewDetailControl), new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        public TwoPaneViewDetailControl()
        {
            InitializeComponent();
        }

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TwoPaneViewDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
