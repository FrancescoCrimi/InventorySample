using CiccioSoft.Inventory.Application;
using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using CiccioSoft.Inventory.Uwp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services
{
    public class OrderServiceUwp /*: IOrderService*/
    {
        private readonly IOrderService orderService;

        public OrderServiceUwp(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public async Task<OrderModel> CreateNewOrderAsync(long customerID)
        {
            Order order = await orderService.CreateNewOrderAsync(customerID);
            OrderModel model = await CreateOrderModelAsync(order, includeAllFields: true);
            if (customerID > 0)
                model.Customer = await CustomerServiceUwp.CreateCustomerModelAsync(order.Customer, includeAllFields: true);
            return model;
        }

        public Task<int> DeleteOrderAsync(OrderModel model)
        {
            var order = new Order { OrderID = model.OrderID };
            return orderService.DeleteOrderAsync(order);
        }

        public Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request)
        {
            return orderService.DeleteOrderRangeAsync(index, length, request);
        }

        public async Task<OrderModel> GetOrderAsync(long id)
        {
            Order order = await orderService.GetOrderAsync(id);
            OrderModel model = await CreateOrderModelAsync(order, includeAllFields: true);
            if (order.Customer != null)
                model.Customer = await CustomerServiceUwp.CreateCustomerModelAsync(order.Customer, true);
            return model;
        }

        public async Task<IList<OrderModel>> GetOrdersAsync(int skip, int take, DataRequest<Order> request)
        {
            var orders = await orderService.GetOrdersAsync(skip, take, request);
            var models = new List<OrderModel>();
            foreach (var item in orders)
            {
                OrderModel model = await CreateOrderModelAsync(item, includeAllFields: false);
                if (item.Customer != null)
                    model.Customer = await CustomerServiceUwp.CreateCustomerModelAsync(item.Customer, false);
                models.Add(model);
            }
            return models;
        }

        public Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            return orderService.GetOrdersCountAsync(request);
        }

        public async Task<int> UpdateOrderAsync(OrderModel model)
        {
            int ret = 0;
            long id = model.OrderID;
            var order = id > 0 ? await orderService.GetOrderAsync(model.OrderID) : new Order();
            if (order != null)
            {
                UpdateOrderFromModel(order, model);
                ret = await orderService.UpdateOrderAsync(order);

                var item = await orderService.GetOrderAsync(id);
                var newmodel = await CreateOrderModelAsync(item, includeAllFields: true);
                model.Merge(newmodel);
            }
            return ret;
        }



        static public async Task<OrderModel> CreateOrderModelAsync(Order source, bool includeAllFields)
        {
            var model = new OrderModel()
            {
                OrderID = source.OrderID,
                CustomerID = source.CustomerID,
                OrderDate = source.OrderDate,
                ShippedDate = source.ShippedDate,
                DeliveredDate = source.DeliveredDate,
                Status = source.Status,
                PaymentType = source.PaymentType,
                TrackingNumber = source.TrackingNumber,
                ShipVia = source.ShipVia,
                ShipAddress = source.ShipAddress,
                ShipCity = source.ShipCity,
                ShipRegion = source.ShipRegion,
                ShipCountryCode = source.ShipCountryCode,
                ShipPostalCode = source.ShipPostalCode,
                ShipPhone = source.ShipPhone,
            };
            if (source.Customer != null)
            {
                model.Customer = await CustomerServiceUwp.CreateCustomerModelAsync(source.Customer, includeAllFields);
            }
            return model;
        }

        private void UpdateOrderFromModel(Order target, OrderModel source)
        {
            target.CustomerID = source.CustomerID;
            target.OrderDate = source.OrderDate;
            target.ShippedDate = source.ShippedDate;
            target.DeliveredDate = source.DeliveredDate;
            target.Status = source.Status;
            target.PaymentType = source.PaymentType;
            target.TrackingNumber = source.TrackingNumber;
            target.ShipVia = source.ShipVia;
            target.ShipAddress = source.ShipAddress;
            target.ShipCity = source.ShipCity;
            target.ShipRegion = source.ShipRegion;
            target.ShipCountryCode = source.ShipCountryCode;
            target.ShipPostalCode = source.ShipPostalCode;
            target.ShipPhone = source.ShipPhone;
        }
    }
}
