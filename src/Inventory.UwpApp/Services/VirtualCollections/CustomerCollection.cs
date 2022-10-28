using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using Inventory.UwpApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.UwpApp.Services
{
    public class CustomerCollection : VirtualRangeCollection<CustomerModel>
    {
        private readonly CustomerServiceUwp customerService;

        public CustomerCollection(CustomerServiceUwp customerService)
        {
            this.customerService = customerService;
        }

        protected override CustomerModel CreateDummyEntity()
        {
            return new CustomerModel() { FirstName = "Dummy Customer" };
        }

        protected async override Task<int> GetCountAsync()
        {
            int result = await customerService.GetCustomersCountAsync(new DataRequest<Customer>());
            return result;
        }

        protected override async Task<IList<CustomerModel>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            //Todo: fix cancellationToken
            var result = await customerService.GetCustomersAsync(skip, take, new DataRequest<Customer>(), dispatcher);
            return result;
        }
    }
}
