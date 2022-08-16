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
using CiccioSoft.Inventory.Domain.Repository;
using CiccioSoft.Inventory.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Application.Impl
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IServiceProvider serviceProvider;

        public OrderItemService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<OrderItem> GetOrderItemAsync(long orderID, int lineID)
        {
            using (var dataService = serviceProvider.GetService<IOrderItemRepository>())
            {
                var item = await dataService.GetOrderItemAsync(orderID, lineID);
                return item;
            }
        }

        public Task<IList<OrderItem>> GetOrderItemsAsync(DataRequest<OrderItem> request)
        {
            // OrderItems are not virtualized
            return GetOrderItemsAsync(0, 100, request);
        }

        public async Task<IList<OrderItem>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request)
        {
            using (var dataService = serviceProvider.GetService<IOrderItemRepository>())
            {
                var items = await dataService.GetOrderItemsAsync(skip, take, request);
                return items;
            }
        }

        public async Task<int> GetOrderItemsCountAsync(DataRequest<OrderItem> request)
        {
            using (var dataService = serviceProvider.GetService<IOrderItemRepository>())
            {
                return await dataService.GetOrderItemsCountAsync(request);
            }
        }

        public async Task<int> UpdateOrderItemAsync(OrderItem orderItem)
        {
            using (var dataService = serviceProvider.GetService<IOrderItemRepository>())
            {
                return await dataService.UpdateOrderItemAsync(orderItem);
            }
        }

        public async Task<int> DeleteOrderItemAsync(OrderItem orderItem)
        {
            using (var dataService = serviceProvider.GetService<IOrderItemRepository>())
            {
                return await dataService.DeleteOrderItemsAsync(orderItem);
            }
        }

        public async Task<int> DeleteOrderItemRangeAsync(int index, int length, DataRequest<OrderItem> request)
        {
            using (var dataService = serviceProvider.GetService<IOrderItemRepository>())
            {
                var items = await dataService.GetOrderItemKeysAsync(index, length, request);
                return await dataService.DeleteOrderItemsAsync(items.ToArray());
            }
        }
    }
}
