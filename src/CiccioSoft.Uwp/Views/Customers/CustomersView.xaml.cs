﻿#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using CiccioSoft.Inventory.Uwp.Services.Infrastructure;
using CiccioSoft.Inventory.Uwp.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CiccioSoft.Inventory.Uwp.Views
{
    public sealed partial class CustomersView : Page
    {
        private readonly WindowService windowService;

        public CustomersView()
        {
            ViewModel = Ioc.Default.GetService<CustomersViewModel>();
            //NavigationService = Ioc.Default.GetService<INavigationService>();
            windowService = Ioc.Default.GetService<WindowService>();
            InitializeComponent();
        }

        public CustomersViewModel ViewModel { get; }
        //public INavigationService NavigationService { get; }
        
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(e.Parameter as CustomerListArgs);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }

        private async void OpenInNewView(object sender, RoutedEventArgs e)
        {
            var args = ViewModel.CustomerList.CreateArgs();
            //args.IsMainView = false;
            await windowService.OpenInNewWindow<CustomersViewModel>(args);
        }

        private async void OpenDetailsInNewView(object sender, RoutedEventArgs e)
        {
            ViewModel.CustomerDetails.CancelEdit();
            if (pivot.SelectedIndex == 0)
            {
                await windowService.OpenInNewWindow<CustomerDetailsViewModel>(ViewModel.CustomerDetails.CreateArgs());
            }
            else
            {
                await windowService.OpenInNewWindow<OrdersViewModel>(ViewModel.CustomerOrders.CreateArgs());
            }
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }
    }
}