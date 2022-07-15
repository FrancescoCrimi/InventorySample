using CiccioSoft.Inventory.Application.Models;
using CiccioSoft.Inventory.Application.Services;
using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services
{
    public class OrderItemServiceUwp : IOrderItemService
    {
        private readonly IOrderItemService orderItemService;

        public OrderItemServiceUwp(IOrderItemService orderItemService)
        {
            this.orderItemService = orderItemService;
        }

        public Task<int> DeleteOrderItemAsync(OrderItemModel model)
        {
            return orderItemService.DeleteOrderItemAsync(model);
        }

        public Task<int> DeleteOrderItemRangeAsync(int index, int length, DataRequest<OrderItem> request)
        {
            return orderItemService.DeleteOrderItemRangeAsync(index, length, request);
        }

        public async Task<OrderItemModel> GetOrderItemAsync(long orderID, int lineID)
        {
            var orderItem = await orderItemService.GetOrderItemAsync(orderID, lineID);
            await ProductServiceUwp.CreateProductModelAsync(orderItem.Product, true);
            return orderItem;
        }

        public async Task<IList<OrderItemModel>> GetOrderItemsAsync(DataRequest<OrderItem> request)
        {
            var orderItems = await orderItemService.GetOrderItemsAsync(request);
            foreach (var item in orderItems)
            {
                await ProductServiceUwp.CreateProductModelAsync(item.Product, false);
            }
            return orderItems;
        }

        public Task<IList<OrderItemModel>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request)
        {
            return orderItemService.GetOrderItemsAsync(skip, take, request);
        }

        public Task<int> GetOrderItemsCountAsync(DataRequest<OrderItem> request)
        {
            return orderItemService.GetOrderItemsCountAsync(request);
        }

        public async Task<int> UpdateOrderItemAsync(OrderItemModel model)
        {
            var rtn = await orderItemService.UpdateOrderItemAsync(model);
            await ProductServiceUwp.CreateProductModelAsync(model.Product, true);
            return rtn;
        }
    }
}
