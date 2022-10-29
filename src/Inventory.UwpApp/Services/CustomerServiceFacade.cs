using CiccioSoft.Inventory.Application;
using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using Inventory.UwpApp.Dto;
using Inventory.UwpApp.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.UwpApp.Services
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
            var customer = new Customer { CustomerID = model.CustomerID };
            return customerService.DeleteCustomerAsync(customer);
        }

        public Task<int> DeleteCustomerRangeAsync(int index, int length, DataRequest<Customer> request)
        {
            return customerService.DeleteCustomerRangeAsync(index, length, request);
        }

        public async Task<CustomerDto> GetCustomerAsync(long id)
        {
            Customer customer = await customerService.GetCustomerAsync(id);
            CustomerDto model = await DtoAssembler.CreateCustomerModelAsync(customer, includeAllFields: true, null);
            return model;
        }

        public async Task<IList<CustomerDto>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request, Windows.UI.Core.CoreDispatcher dispatcher = null)
        {
            var models = new List<CustomerDto>();
            var customers = await customerService.GetCustomersAsync(skip, take, request);
            foreach (var item in customers)
            {
                var dto = await DtoAssembler.CreateCustomerModelAsync(item, includeAllFields: false, dispatcher);
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
            long id = model.CustomerID;
            Customer customer = id > 0 ? await customerService.GetCustomerAsync(model.CustomerID) : new Customer();
            if (customer != null)
            {
                DtoAssembler.UpdateCustomerFromModel(customer, model);
                rtn = await customerService.UpdateCustomerAsync(customer);
                //TODO: fix below
                var item = await customerService.GetCustomerAsync(id);
                var newmodel = await DtoAssembler.CreateCustomerModelAsync(item, includeAllFields: true, null);
                model.Merge(newmodel);
            }
            return rtn;
        }
    }
}
