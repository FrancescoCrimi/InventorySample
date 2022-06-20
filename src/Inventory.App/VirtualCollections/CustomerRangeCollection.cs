using CiccioSoft.Inventory.Data;
using CiccioSoft.Inventory.Uwp.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services
{
    public class CustomerRangeCollection : VirtualRangeCollection<CustomerModel>
    {
        //private readonly ILogger<CustomerList> logger;
        private readonly ICustomerService customerService;
        private DataRequest<Customer> dataRequest;

        public CustomerRangeCollection(
            //ILogger<CustomerList> logger,
                            ICustomerService customerService)
        {
            //this.logger = logger;
            this.customerService = customerService;
        }

        protected override CustomerModel CreateDummyEntity()
        {
            return new CustomerModel() { FirstName = "Suca", LastName = "Forte" };
        }

        protected override async Task<IList<CustomerModel>> FetchRowsAsync(int intskip, int size)
        {
            try
            {
                return await customerService.GetCustomersAsync(intskip, size, dataRequest);
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, "Fetch");
            }
            return null;
        }

        protected override async Task<int> GetCountAsync()
        {
            return await customerService.GetCustomersCountAsync(dataRequest);
        }

        public async Task LoadAsync(DataRequest<Customer> dataRequest)
        {
            this.dataRequest = dataRequest;
            await LoadAsync();
        }
    }
}
