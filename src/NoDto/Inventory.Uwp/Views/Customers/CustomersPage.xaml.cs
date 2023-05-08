#region copyright
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

using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Customers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Inventory.Uwp.Views.Customers
{
    public sealed partial class CustomersPage : Page
    {
        private readonly WindowManagerService _windowService;

        public CustomersPage()
        {
            ViewModel = Ioc.Default.GetService<CustomersViewModel>();
            _windowService = Ioc.Default.GetService<WindowManagerService>();
            InitializeComponent();
        }

        public CustomersViewModel ViewModel { get; }

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
            args.IsMainView = false;
            await _windowService.OpenInNewWindow<CustomersPage>(args);
        }

        private async void OpenDetailsInNewView(object sender, RoutedEventArgs e)
        {
            ViewModel.CustomerDetails.CancelEdit();
            if (pivot.SelectedIndex == 0)
            {
                await _windowService.OpenInNewWindow<CustomerPage>(ViewModel.CustomerDetails.CreateArgs());
            }
            else
            {
                await _windowService.OpenInNewWindow<CustomerPage>(ViewModel.CustomerOrders.CreateArgs());
            }
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }

    }
}
