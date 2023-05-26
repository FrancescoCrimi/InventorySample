// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Inventory.Domain.Aggregates.CustomerAggregate;
using Inventory.Domain.Aggregates.OrderAggregate;
using Inventory.Domain.Aggregates.ProductAggregate;
using Inventory.Domain.AggregatesModel.OrderAggregate;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Customers;
using Inventory.Uwp.ViewModels.Orders;
using Inventory.Uwp.ViewModels.Products;
using Inventory.Uwp.Views.Customers;
using Inventory.Uwp.Views.Orders;
using Inventory.Uwp.Views.Products;
using Microsoft.Extensions.Logging;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.ViewModels.Dashboard
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly NavigationService _navigationService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly AsyncRelayCommand _loadedCommand;
        private readonly RelayCommand _unLoadedCommand;
        private readonly RelayCommand<ItemClickEventArgs> _itemClickCommand;
        private IList<Customer> _customers;
        private IList<Product> _products;
        private IList<Order> _orders;

        public DashboardViewModel(ILogger<DashboardViewModel> logger,
                                  NavigationService navigationService,
                                  ICustomerRepository customerRepository,
                                  IOrderRepository orderRepository,
                                  IProductRepository productRepository)
            : base()
        {
            _logger = logger;
            _navigationService = navigationService;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _loadedCommand = new AsyncRelayCommand(LoadAsync);
            _unLoadedCommand = new RelayCommand(Unload);
            _itemClickCommand = new RelayCommand<ItemClickEventArgs>(ItemClick);
        }

        public IAsyncRelayCommand LoadedCommand => _loadedCommand;

        public ICommand UnLoadedCommand => _unLoadedCommand;

        public ICommand ItemClickCommand => _itemClickCommand;

        public IList<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public IList<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

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
                Customers = await _customerRepository.GetCustomersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadCustomers, ex, "Load Customers");
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
                Orders = await _orderRepository.GetOrdersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadOrders, ex, "Load Orders");
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
                Products = await _productRepository.GetProductsAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadProducts, ex, "Load Products");
            }
        }

        private void ItemClick(ItemClickEventArgs e)
        {
            if (e.ClickedItem is Control control)
            {
                switch (control.Tag as string)
                {
                    case "Customers":
                        _navigationService.Navigate<CustomersPage>(new CustomerListArgs { OrderByDesc = r => r.CreatedOn });
                        break;
                    case "Orders":
                        _navigationService.Navigate<OrdersPage>(new OrderListArgs { OrderByDesc = r => r.OrderDate });
                        break;
                    case "Products":
                        _navigationService.Navigate<ProductsPage>(new ProductListArgs { OrderByDesc = r => r.ListPrice });
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
