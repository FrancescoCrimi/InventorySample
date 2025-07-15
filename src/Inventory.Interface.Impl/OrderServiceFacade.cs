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

using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Interface.Dto;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Interface.Impl
{
    public class OrderServiceFacade : IOrderServiceFacade
    {
        private readonly ILogger<OrderServiceFacade> _logger;
        private readonly OrderService _orderService;

        public OrderServiceFacade(ILogger<OrderServiceFacade> logger,
                                  OrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        public async Task<OrderDto> CreateNewOrderAsync(long customerID)
        {
            var order = await _orderService.CreateNewOrderAsync(customerID);
            OrderDto model = DtoAssembler.DtoFromOrder(order);
            if (customerID > 0)
                model.Customer = DtoAssembler.DtoFromCustomer(order.Customer);
            return model;
        }

        public async Task<int> DeleteOrderAsync(OrderDto model)
        {
            return await _orderService.DeleteOrderAsync(model.Id);
        }

        public async Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request)
        {
            return await _orderService.DeleteOrderRangeAsync(index, length, request);
        }

        public async Task<OrderDto> GetOrderAsync(long id)
        {
            var item = await _orderService.GetOrderAsync(id);
            OrderDto model = DtoAssembler.DtoFromOrder(item);
            if (item.Customer != null)
                model.Customer = DtoAssembler.DtoFromCustomer(item.Customer);
            return model;
        }

        public async Task<List<OrderDto>> GetOrdersAsync(int skip,
                                                         int take,
                                                         DataRequest<Order> request)
        {
            var models = new List<OrderDto>();
            var orders = await _orderService.GetOrdersAsync(skip, take, request);
            foreach (var item in orders)
            {
                OrderDto dto = DtoAssembler.DtoFromOrder(item);
                if (item.Customer != null)
                    dto.Customer = DtoAssembler.DtoFromCustomer(item.Customer);
                models.Add(dto);
            }
            return models;
        }

        public async Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            return await _orderService.GetOrdersCountAsync(request);
        }

        public async Task<int> UpdateOrderAsync(OrderDto model)
        {
            int ret = 0;
            long id = model.Id;
            var order = id > 0 ? await _orderService.GetOrderAsync(model.Id) : new Order();
            if (order != null)
            {
                DtoAssembler.UpdateOrderFromDto(order, model);
                ret = await _orderService.UpdateOrderAsync(order);
                var newmodel = DtoAssembler.DtoFromOrder(order);
                model.Merge(newmodel);
            }
            return ret;

        }
    }
}
