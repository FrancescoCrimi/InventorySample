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
                //var orderItem = new OrderItem { OrderId = model.OrderId, OrderLine = model.OrderLine };
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
                //var model = DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: true);
                //DtoAssembler.DtoFromProduct(item.Product, true);
                return item;
            }
        }

        public async Task<IList<OrderItem>> GetOrderItemsAsync(DataRequest<OrderItem> request)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                //var models = new List<OrderItemDto>();
                var items = await orderItemRepository.GetOrderItemsAsync(0, 100, request);
                //foreach (var item in items)
                //{
                //    var model = DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: false);
                //    model.Product = DtoAssembler.DtoFromProduct(item.Product, false);
                //    models.Add(model);
                //}
                return items;
            }
        }

        public async Task<IList<OrderItem>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                //var models = new List<OrderItemDto>();
                var items = await orderItemRepository.GetOrderItemsAsync(skip, take, request);
                //foreach (var item in items)
                //{
                //    models.Add(DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: false));
                //}
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
                //var orderItem = model.OrderLine > 0 ? await orderItemRepository.GetOrderItemAsync(model.OrderId, model.OrderLine) : new OrderItem();
                //DtoAssembler.UpdateOrderItemFromModel(orderItem, model);
                int suca =  await orderItemRepository.UpdateOrderItemAsync(orderItem);
                //var item = await orderItemRepository.GetOrderItemAsync(orderItem.OrderId, orderItem.OrderLine);
                //var aaaaa = DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: true);
                //model.Merge(aaaaa);
                return suca;
            }
        }
    }
}
