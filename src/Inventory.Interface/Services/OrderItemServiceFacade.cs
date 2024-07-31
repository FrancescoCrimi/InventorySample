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

using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Interface.Dto;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Interface.Services
{
    public class OrderItemServiceFacade
    {
        private readonly ILogger<OrderItemServiceFacade> _logger;
        private readonly OrderItemService _orderItemService;

        public OrderItemServiceFacade(ILogger<OrderItemServiceFacade> logger,
                                      OrderItemService orderItemService)
        {
            _logger = logger;
            _orderItemService = orderItemService;
        }

        public async Task<int> DeleteOrderItemAsync(OrderItemDto model)
        {
            var orderItem = new OrderItem { OrderId = model.OrderId, OrderLine = model.OrderLine };
            int ret = await _orderItemService.DeleteOrderItemAsync(orderItem);
            return ret;
        }

        public async Task<int> DeleteOrderItemRangeAsync(int index, int length, DataRequest<OrderItem> request)
        {
            return await _orderItemService.DeleteOrderItemRangeAsync(index, length, request);
        }

        public async Task<OrderItemDto> GetOrderItemAsync(long orderID, int lineID)
        {
            var item = await _orderItemService.GetOrderItemAsync(orderID, lineID);
            var model = DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: true);
            DtoAssembler.DtoFromProduct(item.Product, true);
            return model;
        }

        public async Task<IList<OrderItemDto>> GetOrderItemsAsync(DataRequest<OrderItem> request)
        {
            var models = new List<OrderItemDto>();
            var items = await _orderItemService.GetOrderItemsAsync(request);
            foreach (var item in items)
            {
                var model = DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: false);
                model.Product = DtoAssembler.DtoFromProduct(item.Product, false);
                models.Add(model);
            }
            return models;
        }

        public async Task<IList<OrderItemDto>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request)
        {
            var models = new List<OrderItemDto>();
            var items = await _orderItemService.GetOrderItemsAsync(skip, take, request);
            foreach (var item in items)
            {
                models.Add(DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: false));
            }
            return models;
        }

        public async Task<int> GetOrderItemsCountAsync(DataRequest<OrderItem> request)
        {
            return await _orderItemService.GetOrderItemsCountAsync(request);
        }

        public async Task<int> UpdateOrderItemAsync(OrderItemDto model)
        {
            var orderItem = model.OrderLine > 0 ? await _orderItemService.GetOrderItemAsync(model.OrderId, model.OrderLine) : new OrderItem();
            DtoAssembler.UpdateOrderItemFromModel(orderItem, model);
            int suca = await _orderItemService.UpdateOrderItemAsync(orderItem);
            var item = await _orderItemService.GetOrderItemAsync(orderItem.OrderId, orderItem.OrderLine);
            var aaaaa = DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: true);
            model.Merge(aaaaa);
            return suca;
        }
    }
}
