using Inventory.Uwp.ViewModels.Customers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Customers
{
    public sealed partial class CustomersListControl : UserControl
    {
        public CustomersListControl()
        {
            InitializeComponent();
        }


        #region ViewModel

        public CustomerListViewModel ViewModel
        {
            get => (CustomerListViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(CustomerListViewModel),
                                        typeof(CustomersListControl),
                                        new PropertyMetadata(null));

        #endregion
    }
}
