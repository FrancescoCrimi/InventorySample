using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Domain.Repository
{
    public interface ICustomerRepository : IDisposable
    {
        Task<Customer> GetCustomerAsync(long id);
        Task<IList<Customer>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request);
        Task<IList<Customer>> GetCustomerKeysAsync(int skip, int take, DataRequest<Customer> request);
        Task<int> GetCustomersCountAsync(DataRequest<Customer> request);
        Task<int> UpdateCustomerAsync(Customer customer);
        Task<int> DeleteCustomersAsync(params Customer[] customers);
    }
}
