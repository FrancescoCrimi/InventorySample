using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Orders;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Inventory.Uwp.Views.Orders
{
    public sealed partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            ViewModel = Ioc.Default.GetService<OrdersViewModel>();
            NavigationService = Ioc.Default.GetService<NavigationService>();
            InitializeComponent();
        }

        public OrdersViewModel ViewModel { get; }
        public NavigationService NavigationService { get; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(e.Parameter as OrderListArgs);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }

        private async void OpenInNewView(object sender, RoutedEventArgs e)
        {
            //await NavigationService.CreateNewViewAsync<OrdersViewModel>(ViewModel.OrderList.CreateArgs());
            await Task.CompletedTask;
        }

        private async void OpenDetailsInNewView(object sender, RoutedEventArgs e)
        {
            ViewModel.OrderDetails.CancelEdit();
            if (pivot.SelectedIndex == 0)
            {
                //await NavigationService.CreateNewViewAsync<OrderDetailsViewModel>(ViewModel.OrderDetails.CreateArgs());
            }
            else
            {
                ////await NavigationService.CreateNewViewAsync<OrderItemsViewModel>(ViewModel.OrderItemList.CreateArgs());
            }
            await Task.CompletedTask;
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }
    }
}
