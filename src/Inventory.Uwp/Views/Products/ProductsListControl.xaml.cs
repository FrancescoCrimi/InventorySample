using Inventory.Uwp.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Inventory.Uwp.Views
{
    public sealed partial class ProductsListControl : UserControl
    {
        public ProductsListControl()
        {
            InitializeComponent();
        }

        #region ViewModel
        public ProductListViewModel ViewModel
        {
            get => (ProductListViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(ProductListViewModel),
                                        typeof(ProductsListControl),
                                        new PropertyMetadata(null));
        #endregion
    }
}
