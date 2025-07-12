// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
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
    public class OrderItemService
    {
        private readonly ILogger<OrderItemService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public OrderItemService(ILogger<OrderItemService> logger,
                                      IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<int> DeleteOrderItemAsync(OrderItem orderItem)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                int ret = await orderItemRepository.DeleteOrderItemsAsync(orderItem);
                return ret;
            }
        }

        public async Task<int> DeleteOrderItemRangeAsync(int index, int length, DataRequest<OrderItem> request)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                var items = await orderItemRepository.GetOrderItemKeysAsync(index, length, request);
                return await orderItemRepository.DeleteOrderItemsAsync(items.ToArray());
            }
        }

        public async Task<OrderItem> GetOrderItemAsync(long orderID, int lineID)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                var item = await orderItemRepository.GetOrderItemAsync(orderID, lineID);
                return item;
            }
        }

        public async Task<IList<OrderItem>> GetOrderItemsAsync(DataRequest<OrderItem> request)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                var items = await orderItemRepository.GetOrderItemsAsync(0, 100, request);
                return items;
            }
        }

        public async Task<IList<OrderItem>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                var items = await orderItemRepository.GetOrderItemsAsync(skip, take, request);
                return items;
            }
        }

        public async Task<int> GetOrderItemsCountAsync(DataRequest<OrderItem> request)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                return await orderItemRepository.GetOrderItemsCountAsync(request);
            }
        }

        public async Task<int> UpdateOrderItemAsync(OrderItem orderItem)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                int suca = await orderItemRepository.UpdateOrderItemAsync(orderItem);
                return suca;
            }
        }
    }
}
