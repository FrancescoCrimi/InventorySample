// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.Aggregates.CustomerAggregate;
using Inventory.Domain.Aggregates.OrderAggregate;
using Inventory.Domain.Aggregates.ProductAggregate;
using Inventory.Domain.AggregatesModel.OrderAggregate;
using Inventory.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public class OrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private static List<Country> _countries;
        private static List<OrderStatus> _orderStatuses;
        private static List<PaymentType> _paymentTypes;
        private static List<Shipper> _shippers;
        private static List<TaxType> _taxTypes;

        public OrderService(ILogger<OrderService> logger,
                            IOrderRepository orderRepository,
                            ICustomerRepository customerRepository,
                            IProductRepository productRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            Task.Run(async () =>
            {
                await GetCountryCodesAsync();
                await GetOrderStatusAsync();
                await GetPaymentTypesAsync();
                await GetShippersAsync();
                await GetTaxTypesAsync();
            });
        }

        public IEnumerable<Country> CountryCodes => _countries;
        public IEnumerable<OrderStatus> OrderStatuses => _orderStatuses;
        public IEnumerable<PaymentType> PaymentTypes => _paymentTypes;
        public IEnumerable<Shipper> Shippers => _shippers;
        public IEnumerable<TaxType> TaxTypes => _taxTypes;


        private async Task GetCountryCodesAsync()
        {
            if (_countries == null)
            {
                try
                {
                    _countries = await _customerRepository.GetCountryCodesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.LoadCountryCodes, ex, "Load CountryCodes");
                }
            }
        }

        private async Task GetOrderStatusAsync()
        {
            if (_orderStatuses == null)
            {
                try
                {
                    _orderStatuses = await _orderRepository.GetOrderStatusAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.LoadOrderStatus, ex, "Load OrderStatus");
                }
            }
        }

        private async Task GetPaymentTypesAsync()
        {
            if (_paymentTypes == null)
            {
                try
                {
                    _paymentTypes = await _orderRepository.GetPaymentTypesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.LoadPaymentTypes, ex, "Load PaymentTypes");
                }
            }
        }

        private async Task GetShippersAsync()
        {
            if (_shippers == null)
            {
                try
                {
                    _shippers = await _orderRepository.GetShippersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.LoadShippers, ex, "Load Shippers");
                }
            }
        }

        private async Task GetTaxTypesAsync()
        {
            if (_taxTypes == null)
            {
                try
                {
                    _taxTypes = await _productRepository.GetTaxTypesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.LoadTaxTypes, ex, "Load TaxTypes");
                }
            }
        }
    }
}
