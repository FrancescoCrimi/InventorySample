using CiccioSoft.Inventory.Data;
using CiccioSoft.Inventory.Data.Models;
using CiccioSoft.Inventory.Data.Services;
using CiccioSoft.Inventory.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services
{
    public class CustomerServiceUwp : ICustomerService
    {
        private readonly ICustomerService customerService;

        public CustomerServiceUwp(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        public Task<int> DeleteCustomerAsync(CustomerModel model)
        {
            return customerService.DeleteCustomerAsync(model);
        }

        public Task<int> DeleteCustomerRangeAsync(int index, int length, DataRequest<Customer> request)
        {
            return customerService.DeleteCustomerRangeAsync(index, length, request);
        }

        public async Task<CustomerModel> GetCustomerAsync(long id)
        {
            var customer = await customerService.GetCustomerAsync(id);
            if (customer != null)
            {
                await CreateCustomerModelAsync(customer, includeAllFields: true);
            }
            return customer;
        }

        public async Task<IList<CustomerModel>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request)
        {
            var customers = await customerService.GetCustomersAsync(skip, take, request);
            foreach (var item in customers)
            {
                await CreateCustomerModelAsync(item, includeAllFields: false);
            }
            return customers;
        }

        public Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            return customerService.GetCustomersCountAsync(request);
        }

        public Task<int> UpdateCustomerAsync(CustomerModel model)
        {
            return customerService.UpdateCustomerAsync(model);
        }


        static public async Task CreateCustomerModelAsync(CustomerModel customer, bool includeAllFields)
        {
            customer.ThumbnailSource = await BitmapTools.LoadBitmapAsync(customer.Thumbnail);
            if (includeAllFields)
                customer.PictureSource = await BitmapTools.LoadBitmapAsync(customer.Picture);
        }
    }
}
