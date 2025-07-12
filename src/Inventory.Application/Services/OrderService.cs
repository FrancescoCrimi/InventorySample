// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public class OrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public OrderService(ILogger<OrderService> logger,
                                  IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<Order> CreateNewOrderAsync(long customerID)
        {
            using (var dataService = _serviceProvider.GetService<ICustomerRepository>())
            {
                var customer = await dataService.GetCustomerAsync(customerID);
                Order order = Order.CreateNewOrder(customer);
                return order;
            }
        }

        public async Task<int> DeleteOrderAsync(long OrderId)
        {
            using (var orderRepository = _serviceProvider.GetService<IOrderRepository>())
            {
                var order = await orderRepository.GetOrderAsync(OrderId);
                return await orderRepository.DeleteOrdersAsync(order);
            }
        }

        public async Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request)
        {
            using (var orderRepository = _serviceProvider.GetService<IOrderRepository>())
            {
                var items = await orderRepository.GetOrderKeysAsync(index, length, request);
                return await orderRepository.DeleteOrdersAsync(items.ToArray());
            }
        }

        public async Task<Order> GetOrderAsync(long OrderId)
        {
            using (var orderRepository = _serviceProvider.GetService<IOrderRepository>())
            {
                var item = await orderRepository.GetOrderAsync(OrderId);
                return item;
            }
        }

        public async Task<IList<Order>> GetOrdersAsync(int skip,
                                                       int take,
                                                       DataRequest<Order> request)
        {
            using (var orderRepository = _serviceProvider.GetService<IOrderRepository>())
            {
                var orders = await orderRepository.GetOrdersAsync(skip, take, request);
                return orders;
            }
        }

        public async Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            using (var orderRepository = _serviceProvider.GetService<IOrderRepository>())
            {
                return await orderRepository.GetOrdersCountAsync(request);
            }
        }

        public async Task<int> UpdateOrderAsync(Order order)
        {
            using (var orderRepository = _serviceProvider.GetService<IOrderRepository>())
            {
                int ret = 0;
                ret = await orderRepository.UpdateOrderAsync(order);
                return ret;
            }
        }
    }
}
