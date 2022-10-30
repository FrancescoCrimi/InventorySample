using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Services;
using Inventory.Uwp.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class CustomerCollection : VirtualRangeCollection<CustomerDto>
    {
        private readonly CustomerServiceFacade customerService;

        public CustomerCollection(CustomerServiceFacade customerService)
        {
            this.customerService = customerService;
        }

        protected override CustomerDto CreateDummyEntity()
        {
            return new CustomerDto() { FirstName = "Dummy Customer" };
        }

        protected override async Task<int> GetCountAsync()
        {
            int result = await customerService.GetCustomersCountAsync(new DataRequest<Customer>());
            return result;
        }

        protected override async Task<IList<CustomerDto>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            //Todo: fix cancellationToken
            var result = await customerService.GetCustomersAsync(skip, take, new DataRequest<Customer>(), dispatcher);
            return result;
        }
    }
}
