using Inventory.Uwp.Dto;
using Inventory.Uwp.ViewModels.Products;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Products
{
    public sealed partial class ProductCardControl : UserControl
    {
        public ProductCardControl()
        {
            InitializeComponent();
        }

        #region ViewModel
        public ProductDetailsViewModel ViewModel
        {
            get { return (ProductDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(ProductDetailsViewModel), typeof(ProductCardControl), new PropertyMetadata(null));
        #endregion

        #region Item
        public ProductDto Item
        {
            get { return (ProductDto)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(ProductDto), typeof(ProductCardControl), new PropertyMetadata(null));
        #endregion
    }
}
