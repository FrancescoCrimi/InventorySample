using Inventory.Uwp.ViewModels.Products;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Products
{
    public sealed partial class ProductDetailsControl : UserControl
    {
        public ProductDetailsControl()
        {
            InitializeComponent();
        }

        #region ViewModel
        public ProductDetailsViewModel ViewModel
        {
            get { return (ProductDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(ProductDetailsViewModel), typeof(ProductDetailsControl), new PropertyMetadata(null));
        #endregion

        public void SetFocus()
        {
            details.SetFocus();
        }
    }
}
