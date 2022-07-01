using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Domain.Repository
{
    public interface IOrderItemRepository : IDisposable
    {
        Task<OrderItem> GetOrderItemAsync(long orderID, int orderLine);
        Task<IList<OrderItem>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request);
        Task<IList<OrderItem>> GetOrderItemKeysAsync(int skip, int take, DataRequest<OrderItem> request);
        Task<int> GetOrderItemsCountAsync(DataRequest<OrderItem> request);
        Task<int> UpdateOrderItemAsync(OrderItem orderItem);
        Task<int> DeleteOrderItemsAsync(params OrderItem[] orderItems);
    }
}
