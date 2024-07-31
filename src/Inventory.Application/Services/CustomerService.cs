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

using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public class CustomerService
    {
        private readonly ILogger<CustomerService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CustomerService(ILogger<CustomerService> logger,
                               IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<int> DeleteCustomerAsync(long CustomerId)
        {
            using (var customerRepository = _serviceProvider.GetService<ICustomerRepository>())
            {
                var customer = await customerRepository.GetCustomerAsync(CustomerId);
                return await customerRepository.DeleteCustomersAsync(customer);
            }
        }

        public async Task<int> DeleteCustomerRangeAsync(int index,
                                                        int length,
                                                        DataRequest<Customer> request)
        {
            using (var customerRepository = _serviceProvider.GetService<ICustomerRepository>())
            {
                var items = await customerRepository.GetCustomerKeysAsync(index, length, request);
                return await customerRepository.DeleteCustomersAsync(items.ToArray());
            }
        }

        public async Task<Customer> GetCustomerAsync(long CustomerId)
        {
            using (var customerRepository = _serviceProvider.GetService<ICustomerRepository>())
            {
                var customer = await customerRepository.GetCustomerAsync(CustomerId);
                //CustomerDto model = DtoAssembler.DtoFromCustomer(customer);
                return customer;
            }
        }

        public async Task<IList<Customer>> GetCustomersAsync(int skip,
                                                               int take,
                                                               DataRequest<Customer> request
            //, Windows.UI.Core.CoreDispatcher dispatcher = null
            )
        {
            using (var customerRepository = _serviceProvider.GetService<ICustomerRepository>())
            {
                //var models = new List<CustomerDto>();
                var items = await customerRepository.GetCustomersAsync(skip, take, request);
                //foreach (var item in items)
                //{
                //    var dto = DtoAssembler.DtoFromCustomer(item);
                //    models.Add(dto);
                //}
                return items;
            }
        }

        public async Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            using (var customerRepository = _serviceProvider.GetService<ICustomerRepository>())
            {
                return await customerRepository.GetCustomersCountAsync(request);
            }
        }

        public async Task<int> UpdateCustomerAsync(Customer customer)
        {
            using (var customerRepository = _serviceProvider.GetService<ICustomerRepository>())
            {
                int rtn = 0;
                //long id = model.Id;
                //Customer customer = id > 0 ? await customerRepository.GetCustomerAsync(model.Id) : new Customer();
                if (customer != null)
                {
                    //DtoAssembler.UpdateCustomerFromDto(customer, model);
                    rtn = await customerRepository.UpdateCustomerAsync(customer);
                    //TODO: fix below
                    //var item = await customerRepository.GetCustomerAsync(id);
                    //var newmodel = DtoAssembler.DtoFromCustomer(item);
                    //model.Merge(newmodel);
                }
                return rtn;
            }
        }
    }
}
