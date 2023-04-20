using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Domain.Repository
{
    public interface IOrderRepository : IDisposable
    {
        Task<Order> GetOrderAsync(long id);
        Task<IList<Order>> GetOrdersAsync(int skip, int take, DataRequest<Order> request);
        Task<IList<Order>> GetOrderKeysAsync(int skip, int take, DataRequest<Order> request);
        Task<int> GetOrdersCountAsync(DataRequest<Order> request);
        Task<int> UpdateOrderAsync(Order order);
        Task<int> DeleteOrdersAsync(params Order[] orders);
    }
}
