using CiccioSoft.Inventory.Application;
using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using CiccioSoft.Inventory.Uwp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services
{
    public class OrderItemServiceUwp /*: IOrderItemService*/
    {
        private readonly IOrderItemService orderItemService;

        public OrderItemServiceUwp(IOrderItemService orderItemService)
        {
            this.orderItemService = orderItemService;
        }

        public async Task<int> DeleteOrderItemAsync(OrderItemModel model)
        {
            var orderItem = new OrderItem { OrderID = model.OrderID, OrderLine = model.OrderLine };
            int ret = await orderItemService.DeleteOrderItemAsync(orderItem);
            return ret;
        }

        public Task<int> DeleteOrderItemRangeAsync(int index, int length, DataRequest<OrderItem> request)
        {
            return orderItemService.DeleteOrderItemRangeAsync(index, length, request);
        }

        public async Task<OrderItemModel> GetOrderItemAsync(long orderID, int lineID)
        {
            var orderItem = await orderItemService.GetOrderItemAsync(orderID, lineID);
            var model = await CreateOrderItemModelAsync(orderItem, includeAllFields: true);
            await ProductServiceUwp.CreateProductModelAsync(orderItem.Product, true);
            return model;
        }

        public async Task<IList<OrderItemModel>> GetOrderItemsAsync(DataRequest<OrderItem> request)
        {
            var models = new List<OrderItemModel>();
            var orderItems = await orderItemService.GetOrderItemsAsync(request);
            foreach (var item in orderItems)
            {
                var model = await CreateOrderItemModelAsync(item, includeAllFields: false);
                model.Product = await ProductServiceUwp.CreateProductModelAsync(item.Product, false);
                models.Add(model);
            }
            return models;
        }

        public async Task<IList<OrderItemModel>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request)
        {
            var models = new List<OrderItemModel>();
            var items = await orderItemService.GetOrderItemsAsync(skip, take, request);
            foreach (var item in items)
            {
                models.Add(await CreateOrderItemModelAsync(item, includeAllFields: false));
            }
            return models;
        }

        public Task<int> GetOrderItemsCountAsync(DataRequest<OrderItem> request)
        {
            return orderItemService.GetOrderItemsCountAsync(request);
        }

        public async Task<int> UpdateOrderItemAsync(OrderItemModel model)
        {
            var orderItem = model.OrderLine > 0 ? await orderItemService.GetOrderItemAsync(model.OrderID, model.OrderLine) : new OrderItem();
            UpdateOrderItemFromModel(orderItem, model);
            int suca = await orderItemService.UpdateOrderItemAsync(orderItem);
            var item = await orderItemService.GetOrderItemAsync(orderItem.OrderID, orderItem.OrderLine);
            var aaaaa = await CreateOrderItemModelAsync(item, includeAllFields: true);
            model.Merge(aaaaa);
            return suca;
        }


        public static async Task<OrderItemModel> CreateOrderItemModelAsync(OrderItem source, bool includeAllFields)
        {
            var model = new OrderItemModel()
            {
                OrderID = source.OrderID,
                OrderLine = source.OrderLine,
                ProductID = source.ProductID,
                Quantity = source.Quantity,
                UnitPrice = source.UnitPrice,
                Discount = source.Discount,
                TaxType = source.TaxType,
                Product = await ProductServiceUwp.CreateProductModelAsync(source.Product, includeAllFields)
            };
            return model;
        }

        private void UpdateOrderItemFromModel(OrderItem target, OrderItemModel source)
        {
            target.OrderID = source.OrderID;
            target.OrderLine = source.OrderLine;
            target.ProductID = source.ProductID;
            target.Quantity = source.Quantity;
            target.UnitPrice = source.UnitPrice;
            target.Discount = source.Discount;
            target.TaxType = source.TaxType;
        }
    }
}
