using Inventory.Uwp.Dto;
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

namespace Inventory.Uwp.Views.Orders
{
    public sealed partial class OrdersCardControl : UserControl
    {
        public OrdersCardControl()
        {
            this.InitializeComponent();
        }

        #region Item
        public OrderDto Item
        {
            get { return (OrderDto)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(OrderDto), typeof(OrdersCardControl), new PropertyMetadata(null));
        #endregion
    }
}
