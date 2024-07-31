// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Interface.Dto;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Interface.Services
{
    public class CustomerServiceFacade
    {
        private readonly ILogger<CustomerServiceFacade> _logger;
        private readonly CustomerService _customerService;

        public CustomerServiceFacade(ILogger<CustomerServiceFacade> logger,
                               CustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        public async Task<int> DeleteCustomerAsync(CustomerDto model)
        {
            return await _customerService.DeleteCustomerAsync(model.Id);
        }

        public async Task<int> DeleteCustomerRangeAsync(int index,
                                                        int length,
                                                        DataRequest<Customer> request)
        {
            return await _customerService.DeleteCustomerRangeAsync(index, length, request);
        }

        public async Task<CustomerDto> GetCustomerAsync(long id)
        {
            var customer = await _customerService.GetCustomerAsync(id);
            CustomerDto model = DtoAssembler.DtoFromCustomer(customer);
            return model;
        }

        public async Task<List<CustomerDto>> GetCustomersAsync(int skip,
                                                               int take,
                                                               DataRequest<Customer> request)
        {
            var models = new List<CustomerDto>();
            var items = await _customerService.GetCustomersAsync(skip, take, request);
            foreach (var item in items)
            {
                var dto = DtoAssembler.DtoFromCustomer(item);
                models.Add(dto);
            }
            return models;
        }

        public async Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            return await _customerService.GetCustomersCountAsync(request);
        }

        public async Task<int> UpdateCustomerAsync(CustomerDto model)
        {
            int rtn = 0;
            long id = model.Id;
            Customer customer = id > 0 ? await _customerService.GetCustomerAsync(model.Id) : new Customer();
            if (customer != null)
            {
                DtoAssembler.UpdateCustomerFromDto(customer, model);
                rtn = await _customerService.UpdateCustomerAsync(customer);
                //TODO: fix below
                var item = await _customerService.GetCustomerAsync(id);
                var newmodel = DtoAssembler.DtoFromCustomer(item);
                model.Merge(newmodel);
            }
            return rtn;
        }
    }
}
