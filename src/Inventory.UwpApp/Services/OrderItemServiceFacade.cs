using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.UwpApp.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.UwpApp.Services
{
    public class OrderItemServiceFacade /*: IOrderItemService*/
    {
        private readonly IOrderItemService orderItemService;

        public OrderItemServiceFacade(IOrderItemService orderItemService)
        {
            this.orderItemService = orderItemService;
        }

        public async Task<int> DeleteOrderItemAsync(OrderItemDto model)
        {
            var orderItem = new OrderItem { OrderID = model.OrderID, OrderLine = model.OrderLine };
            int ret = await orderItemService.DeleteOrderItemAsync(orderItem);
            return ret;
        }

        public Task<int> DeleteOrderItemRangeAsync(int index, int length, DataRequest<OrderItem> request)
        {
            return orderItemService.DeleteOrderItemRangeAsync(index, length, request);
        }

        public async Task<OrderItemDto> GetOrderItemAsync(long orderID, int lineID)
        {
            var orderItem = await orderItemService.GetOrderItemAsync(orderID, lineID);
            var model = await DtoAssembler.CreateOrderItemModelAsync(orderItem, includeAllFields: true);
            await DtoAssembler.CreateProductModelAsync(orderItem.Product, true, null);
            return model;
        }

        public async Task<IList<OrderItemDto>> GetOrderItemsAsync(DataRequest<OrderItem> request)
        {
            var models = new List<OrderItemDto>();
            var orderItems = await orderItemService.GetOrderItemsAsync(request);
            foreach (var item in orderItems)
            {
                var model = await DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: false);
                model.Product = await DtoAssembler.CreateProductModelAsync(item.Product, false, null);
                models.Add(model);
            }
            return models;
        }

        public async Task<IList<OrderItemDto>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request)
        {
            var models = new List<OrderItemDto>();
            var items = await orderItemService.GetOrderItemsAsync(skip, take, request);
            foreach (var item in items)
            {
                models.Add(await DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: false));
            }
            return models;
        }

        public Task<int> GetOrderItemsCountAsync(DataRequest<OrderItem> request)
        {
            return orderItemService.GetOrderItemsCountAsync(request);
        }

        public async Task<int> UpdateOrderItemAsync(OrderItemDto model)
        {
            var orderItem = model.OrderLine > 0 ? await orderItemService.GetOrderItemAsync(model.OrderID, model.OrderLine) : new OrderItem();
            DtoAssembler.UpdateOrderItemFromModel(orderItem, model);
            int suca = await orderItemService.UpdateOrderItemAsync(orderItem);
            var item = await orderItemService.GetOrderItemAsync(orderItem.OrderID, orderItem.OrderLine);
            var aaaaa = await DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: true);
            model.Merge(aaaaa);
            return suca;
        }
    }
}
