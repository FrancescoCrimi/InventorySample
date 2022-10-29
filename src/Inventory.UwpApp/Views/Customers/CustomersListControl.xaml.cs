using Inventory.UwpApp.ViewModels;
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

namespace Inventory.UwpApp.Views.Customers
{
    public sealed partial class CustomersListControl : UserControl
    {
        public CustomersListControl()
        {
            this.InitializeComponent();
        }


        #region ViewModel

        public CustomerListViewModel ViewModel
        {
            get
            {
                return (CustomerListViewModel)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(CustomerListViewModel), typeof(CustomersListControl), new PropertyMetadata(null));

        #endregion
    }
}
