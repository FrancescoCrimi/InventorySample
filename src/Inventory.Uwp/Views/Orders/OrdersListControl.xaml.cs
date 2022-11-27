using Inventory.Uwp.ViewModels.Orders;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Orders
{
    public sealed partial class OrdersListControl : UserControl
    {
        public OrdersListControl()
        {
            this.InitializeComponent();
        }

        #region ViewModel
        public OrderListViewModel ViewModel
        {
            get { return (OrderListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(OrderListViewModel), typeof(OrdersListControl), new PropertyMetadata(null));
        #endregion
    }
}
