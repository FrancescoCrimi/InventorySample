using CommunityToolkit.Mvvm.DependencyInjection;
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

namespace Inventory.UwpApp.Views.Customers
{
    public sealed partial class CustomersPage : Page
    {
        public CustomersPage()
        {
            this.InitializeComponent();
            ViewModel = Ioc.Default.GetService<CustomersViewModel>();
            //DataContext = ViewModel;
        }

        public CustomersViewModel ViewModel { get; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(e.Parameter as CustomerListArgs);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }

        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void OnDoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }





        private  void OpenInNewView(object sender, RoutedEventArgs e)
        {
            var args = ViewModel.CustomerList.CreateArgs();
            //args.IsMainView = false;
            //await windowService.OpenInNewWindow<CustomersViewModel>(args);
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }

    }
}
