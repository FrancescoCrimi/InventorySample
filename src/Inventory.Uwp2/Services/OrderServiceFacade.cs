using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services
{
    public class OrderServiceFacade /*: IOrderService*/
    {
        private readonly IOrderService orderService;

        public OrderServiceFacade(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public async Task<OrderDto> CreateNewOrderAsync(long customerID)
        {
            Order order = await orderService.CreateNewOrderAsync(customerID);
            OrderDto model = await DtoAssembler.CreateOrderModelAsync(order, includeAllFields: true, null);
            if (customerID > 0)
                model.Customer = await DtoAssembler.CreateCustomerModelAsync(order.Customer, includeAllFields: true);
            return model;
        }

        public Task<int> DeleteOrderAsync(OrderDto model)
        {
            var order = new Order { OrderID = model.OrderID };
            return orderService.DeleteOrderAsync(order);
        }

        public Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request)
        {
            return orderService.DeleteOrderRangeAsync(index, length, request);
        }

        public async Task<OrderDto> GetOrderAsync(long id)
        {
            Order order = await orderService.GetOrderAsync(id);
            OrderDto model = await DtoAssembler.CreateOrderModelAsync(order, includeAllFields: true, null);
            if (order.Customer != null)
                model.Customer = await DtoAssembler.CreateCustomerModelAsync(order.Customer, true);
            return model;
        }

        public async Task<List<OrderDto>> GetOrdersAsync(int skip, int take, DataRequest<Order> request, Windows.UI.Core.CoreDispatcher dispatcher = null)
        {
            var orders = await orderService.GetOrdersAsync(skip, take, request);
            var models = new List<OrderDto>();
            foreach (var item in orders)
            {
                OrderDto dto = await DtoAssembler.CreateOrderModelAsync(item, includeAllFields: false, dispatcher);
                if (item.Customer != null)
                    dto.Customer = await DtoAssembler.CreateCustomerModelAsync(item.Customer, false);
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
            long id = model.OrderID;
            var order = id > 0 ? await orderService.GetOrderAsync(model.OrderID) : new Order();
            if (order != null)
            {
                DtoAssembler.UpdateOrderFromModel(order, model);
                ret = await orderService.UpdateOrderAsync(order);

                var item = await orderService.GetOrderAsync(id);
                var newmodel = await DtoAssembler.CreateOrderModelAsync(item, includeAllFields: true, null);
                model.Merge(newmodel);
            }
            return ret;
        }
    }
}
