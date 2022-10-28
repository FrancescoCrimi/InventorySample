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

using CiccioSoft.Inventory.Uwp.Services.Infrastructure;
using CiccioSoft.Inventory.Uwp.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CiccioSoft.Inventory.Uwp.Views
{
    public sealed partial class OrderItemsView : Page
    {
        private readonly WindowService windowService;

        public OrderItemsView()
        {
            ViewModel = Ioc.Default.GetService<OrderItemsViewModel>();
            //NavigationService = Ioc.Default.GetService<INavigationService>();
            windowService = Ioc.Default.GetService<WindowService>();
            InitializeComponent();
        }

        public OrderItemsViewModel ViewModel { get; }
        //public INavigationService NavigationService { get; }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(e.Parameter as OrderItemListArgs);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }

        private async void OpenInNewView(object sender, RoutedEventArgs e)
        {
            await windowService.OpenInNewWindow<OrderItemsViewModel>(ViewModel.OrderItemList.CreateArgs());
        }

        private async void OpenDetailsInNewView(object sender, RoutedEventArgs e)
        {
            ViewModel.OrderItemDetails.CancelEdit();
            await windowService.OpenInNewWindow<OrderItemDetailsViewModel>(ViewModel.OrderItemDetails.CreateArgs());
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }
    }
}
