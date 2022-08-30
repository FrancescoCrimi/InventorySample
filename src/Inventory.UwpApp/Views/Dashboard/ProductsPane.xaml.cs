﻿using Inventory.UwpApp.Models;
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

// Il modello di elemento Controllo utente è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234236

namespace Inventory.UwpApp.Views.Dashboard
{
    public sealed partial class ProductsPane : UserControl
    {
        public ProductsPane()
        {
            this.InitializeComponent();
        }

        #region ItemsSource
        public IList<ProductModel> ItemsSource
        {
            get { return (IList<ProductModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IList<ProductModel>), typeof(ProductsPane), new PropertyMetadata(null));
        #endregion
    }
}