using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services
{
    public class CustomerServiceFacade /*: ICustomerService*/
    {
        private readonly ICustomerService customerService;

        public CustomerServiceFacade(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        public Task<int> DeleteCustomerAsync(CustomerDto model)
        {
            var customer = new Customer { Id = model.Id };
            return customerService.DeleteCustomerAsync(customer);
        }

        public Task<int> DeleteCustomerRangeAsync(int index, int length, DataRequest<Customer> request)
        {
            return customerService.DeleteCustomerRangeAsync(index, length, request);
        }

        public async Task<CustomerDto> GetCustomerAsync(long id)
        {
            Customer customer = await customerService.GetCustomerAsync(id);
            CustomerDto model = DtoAssembler.DtoFromCustomer(customer);
            return model;
        }

        public async Task<List<CustomerDto>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request, Windows.UI.Core.CoreDispatcher dispatcher = null)
        {
            var models = new List<CustomerDto>();
            var customers = await customerService.GetCustomersAsync(skip, take, request);
            foreach (var item in customers)
            {
                var dto = DtoAssembler.DtoFromCustomer(item);
                models.Add(dto);
            }
            return models;
        }

        public Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            return customerService.GetCustomersCountAsync(request);
        }

        public async Task<int> UpdateCustomerAsync(CustomerDto model)
        {
            int rtn = 0;
            long id = model.Id;
            Customer customer = id > 0 ? await customerService.GetCustomerAsync(model.Id) : new Customer();
            if (customer != null)
            {
                DtoAssembler.UpdateCustomerFromDto(customer, model);
                rtn = await customerService.UpdateCustomerAsync(customer);
                //TODO: fix below
                var item = await customerService.GetCustomerAsync(id);
                var newmodel = DtoAssembler.DtoFromCustomer(item);
                model.Merge(newmodel);
            }
            return rtn;
        }
    }
}
