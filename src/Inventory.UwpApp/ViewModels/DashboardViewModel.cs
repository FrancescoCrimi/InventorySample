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

using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using CommunityToolkit.Mvvm.Input;
using Inventory.UwpApp.Models;
using Inventory.UwpApp.Services;
using Inventory.UwpApp.Views;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Inventory.UwpApp.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly ILogger<DashboardViewModel> logger;
        private readonly NavigationService navigationService;
        private readonly CustomerServiceUwp customerService;
        private readonly OrderServiceUwp orderService;
        private readonly ProductServiceUwp productService;

        public DashboardViewModel(ILogger<DashboardViewModel> logger,
                                  NavigationService navigationService,
                                  CustomerServiceUwp customerService,
                                  OrderServiceUwp orderService,
                                  ProductServiceUwp productService)
            : base()
        {
            this.logger = logger;
            this.navigationService = navigationService;
            this.customerService = customerService;
            this.orderService = orderService;
            this.productService = productService;
        }

        private AsyncRelayCommand loadedCommand = null;
        public IAsyncRelayCommand LoadedCommand => loadedCommand ??
            (loadedCommand = new AsyncRelayCommand(LoadAsync));

        private RelayCommand unLoadedCommand = null;
        public ICommand UnLoadedCommand => unLoadedCommand ??
            (unLoadedCommand = new RelayCommand(Unload));

        private RelayCommand<ItemClickEventArgs> itemClickCommand = null;
        public ICommand ItemClickCommand => itemClickCommand ??
                (itemClickCommand = new RelayCommand<ItemClickEventArgs>(ItemClick));

        private IList<CustomerModel> _customers = null;
        public IList<CustomerModel> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        private IList<ProductModel> _products = null;
        public IList<ProductModel> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        private IList<OrderModel> _orders = null;
        public IList<OrderModel> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        public async Task LoadAsync()
        {
            StartStatusMessage("Loading dashboard...");
            await LoadCustomersAsync();
            await LoadOrdersAsync();
            await LoadProductsAsync();
            EndStatusMessage("Dashboard loaded");
        }
        public void Unload()
        {
            Customers = null;
            Products = null;
            Orders = null;
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var request = new DataRequest<Customer>
                {
                    OrderByDesc = r => r.CreatedOn
                };
                Customers = await customerService.GetCustomersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Customers");
            }
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                var request = new DataRequest<Order>
                {
                    OrderByDesc = r => r.OrderDate
                };
                Orders = await orderService.GetOrdersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Orders");
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var request = new DataRequest<Product>
                {
                    OrderByDesc = r => r.CreatedOn
                };
                Products = await productService.GetProductsAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Products");
            }
        }

        private void ItemClick(ItemClickEventArgs e)
        {
            if (e.ClickedItem is Control control)
            {
                switch (control.Tag as String)
                {
                    case "Customers":
                        navigationService.Navigate<CustomersPage>(new CustomerListArgs { OrderByDesc = r => r.CreatedOn });
                        break;
                    case "Orders":
                        navigationService.Navigate<OrdersPage>(new OrderListArgs { OrderByDesc = r => r.OrderDate });
                        break;
                    case "Products":
                        navigationService.Navigate<ProductsPage>(new ProductListArgs { OrderByDesc = r => r.ListPrice });
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
