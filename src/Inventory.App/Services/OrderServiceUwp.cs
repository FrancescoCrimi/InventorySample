using CiccioSoft.Inventory.Data;
using CiccioSoft.Inventory.Data.Models;
using CiccioSoft.Inventory.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services
{
    public class OrderServiceUwp : IOrderService
    {
        private readonly IOrderService orderService;

        public OrderServiceUwp(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public async Task<OrderModel> CreateNewOrderAsync(long customerID)
        {
            var order = await orderService.CreateNewOrderAsync(customerID);
            if (customerID > 0)
                 await CustomerServiceUwp.CreateCustomerModelAsync(order.Customer, includeAllFields: true);
            return order;
        }

        public Task<int> DeleteOrderAsync(OrderModel model)
        {
            return orderService.DeleteOrderAsync(model);
        }

        public Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request)
        {
            return orderService.DeleteOrderRangeAsync(index, length, request);
        }

        public async Task<OrderModel> GetOrderAsync(long id)
        {
            var order = await orderService.GetOrderAsync(id);
            if (order.Customer != null)
                await CustomerServiceUwp.CreateCustomerModelAsync(order.Customer, true);
            return order;
        }

        public async Task<IList<OrderModel>> GetOrdersAsync(int skip, int take, DataRequest<Order> request)
        {
            var orders = await orderService.GetOrdersAsync(skip, take, request);
            foreach (var item in orders)
            {
                if (item.Customer != null)
                   await  CustomerServiceUwp.CreateCustomerModelAsync(item.Customer, false);
            }
            return orders;
        }

        public Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            return orderService.GetOrdersCountAsync(request);
        }

        public Task<int> UpdateOrderAsync(OrderModel model)
        {
            return orderService.UpdateOrderAsync(model);
        }
    }
}
