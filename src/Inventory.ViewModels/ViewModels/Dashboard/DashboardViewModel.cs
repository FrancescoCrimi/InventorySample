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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Inventory.Data;
using Inventory.Models;
using Inventory.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Inventory.ViewModels
{
    public class DashboardViewModel : ObservableRecipient
    {
        public DashboardViewModel(ILogger<DashboardViewModel> logger,
            INavigationService navigationService,
            ICustomerService customerService,
                                  IOrderService orderService,
                                  IProductService productService,
                                  ICommonServices commonServices)
            //: base(commonServices)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            CustomerService = customerService;
            OrderService = orderService;
            ProductService = productService;
        }

        public ICustomerService CustomerService { get; }
        public IOrderService OrderService { get; }
        public IProductService ProductService { get; }

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
        private readonly ILogger<DashboardViewModel> logger;
        private readonly INavigationService navigationService;

        public IList<OrderModel> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        public async Task LoadAsync()
        {
            //StartStatusMessage("Loading dashboard...");
            await LoadCustomersAsync();
            await LoadOrdersAsync();
            await LoadProductsAsync();
            //EndStatusMessage("Dashboard loaded");
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
                Customers = await CustomerService.GetCustomersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                //LogException("Dashboard", "Load Customers", ex);
                logger.LogCritical(ex, "Load Customers");
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
                Orders = await OrderService.GetOrdersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                //LogException("Dashboard", "Load Orders", ex);
                logger.LogCritical(ex, "Load Orders");
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
                Products = await ProductService.GetProductsAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                //LogException("Dashboard", "Load Products", ex);
                logger.LogCritical(ex, "Load Products");
            }
        }

        public void ItemSelected(string item)
        {
            switch (item)
            {
                case "Customers":
                    navigationService.Navigate<CustomersViewModel>(new CustomerListArgs { OrderByDesc = r => r.CreatedOn });
                    break;
                case "Orders":
                    navigationService.Navigate<OrdersViewModel>(new OrderListArgs { OrderByDesc = r => r.OrderDate });
                    break;
                case "Products":
                    navigationService.Navigate<ProductsViewModel>(new ProductListArgs { OrderByDesc = r => r.ListPrice });
                    break;
                default:
                    break;
            }
        }
    }
}
