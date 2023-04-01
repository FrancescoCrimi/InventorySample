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

using CommunityToolkit.Mvvm.Input;
using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
//using Inventory.Uwp.ViewModels.Customers;
//using Inventory.Uwp.ViewModels.Orders;
//using Inventory.Uwp.ViewModels.Products;
//using Inventory.Uwp.Views.Customers;
//using Inventory.Uwp.Views.Orders;
//using Inventory.Uwp.Views.Products;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.ViewModels.Dashboard
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly ILogger<DashboardViewModel> logger;
        //private readonly NavigationService navigationService;
        private readonly CustomerServiceFacade customerService;
        private readonly ICustomerService _customerService;

        private readonly OrderServiceFacade orderService;
        private readonly IOrderService _orderService;
        private readonly ProductServiceFacade productService;
        private readonly IProductService _productService;

        public DashboardViewModel(ILogger<DashboardViewModel> logger,
                                  //NavigationService navigationService,
                                  CustomerServiceFacade customerService,
                                  ICustomerService customerService1,
                                  OrderServiceFacade orderService,
                                  IOrderService orderService1,
                                  ProductServiceFacade productService,
                                  IProductService productService1
            )
            : base()
        {
            this.logger = logger;
            //this.navigationService = navigationService;
            this.customerService = customerService;
            this._customerService = customerService1;
            this.orderService = orderService;
            this._orderService = orderService1;
            this.productService = productService;
            this._productService = productService1;
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

        private IList<Customer> _customers = null;
        public IList<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        private IList<Product> _products = null;
        public IList<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        private IList<Order> _orders = null;
        public IList<Order> Orders
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
            //Products = null;
            //Orders = null;
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var request = new DataRequest<Customer>
                {
                    OrderByDesc = r => r.CreatedOn
                };
                Customers = await _customerService.GetCustomersAsync(0, 5, request);
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
                Orders = await _orderService.GetOrdersAsync(0, 5, request);
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
                Products = await _productService.GetProductsAsync(0, 5, request);
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
                switch (control.Tag as string)
                {
                    //case "Customers":
                    //    navigationService.Navigate<CustomersPage>(new CustomerListArgs { OrderByDesc = r => r.CreatedOn });
                    //    break;
                    //case "Orders":
                    //    navigationService.Navigate<OrdersPage>(new OrderListArgs { OrderByDesc = r => r.OrderDate });
                    //    break;
                    //case "Products":
                    //    navigationService.Navigate<ProductsPage>(new ProductListArgs { OrderByDesc = r => r.ListPrice });
                    //    break;
                    //default:
                    //    break;
                }
            }
        }
    }
}
