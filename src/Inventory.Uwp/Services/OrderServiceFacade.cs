using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Dto;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.WebUI;

namespace Inventory.Uwp.Services
{
    public class OrderServiceFacade /*: IOrderService*/
    {
        private readonly IOrderService orderService;
        private readonly IServiceProvider serviceProvider;

        public OrderServiceFacade(IOrderService orderService, IServiceProvider serviceProvider)
        {
            this.orderService = orderService;
            this.serviceProvider = serviceProvider;
        }

        public async Task<OrderDto> CreateNewOrderAsync(long customerID)
        {
            Order order = await orderService.CreateNewOrderAsync(customerID);
            OrderDto model = DtoAssembler.DtoFromOrder(order);
            if (customerID > 0)
                model.Customer = DtoAssembler.DtoFromCustomer(order.Customer);
            return model;
        }

        public Task<int> DeleteOrderAsync(OrderDto model)
        {
            var order = new Order { Id = model.Id };
            return orderService.DeleteOrderAsync(order);
        }

        public Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request)
        {
            return orderService.DeleteOrderRangeAsync(index, length, request);
        }

        public async Task<OrderDto> GetOrderAsync(long id)
        {
            //var model = new OrderDto();
            //using(var repo = serviceProvider.GetService<IOrderRepository>())
            //{
            //    var item = await repo.GetOrderAsync(id);
            //}

            Order order = await orderService.GetOrderAsync(id);
            OrderDto model = DtoAssembler.DtoFromOrder(order);
            if (order.Customer != null)
                model.Customer = DtoAssembler.DtoFromCustomer(order.Customer);

            return model;
        }

        public async Task<List<OrderDto>> GetOrdersAsync(int skip, int take, DataRequest<Order> request, Windows.UI.Core.CoreDispatcher dispatcher = null)
        {
            var orders = await orderService.GetOrdersAsync(skip, take, request);
            var models = new List<OrderDto>();
            foreach (var item in orders)
            {
                OrderDto dto = DtoAssembler.DtoFromOrder(item);
                if (item.Customer != null)
                    dto.Customer = DtoAssembler.DtoFromCustomer(item.Customer);
                models.Add(dto);
            }
            return models;
        }

        public Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            return orderService.GetOrdersCountAsync(request);
        }

        public async Task<int> UpdateOrderAsync(OrderDto model)
        {
            int ret = 0;
            long id = model.Id;
            var order = id > 0 ? await orderService.GetOrderAsync(model.Id) : new Order();
            if (order != null)
            {
                DtoAssembler.UpdateOrderFromDto(order, model);
                ret = await orderService.UpdateOrderAsync(order);

                var item = await orderService.GetOrderAsync(id);
                var newmodel = DtoAssembler.DtoFromOrder(item);
                model.Merge(newmodel);
            }
            return ret;
        }
    }
}
