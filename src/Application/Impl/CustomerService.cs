#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Domain.Repository;
using CiccioSoft.Inventory.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Application.Impl
{
    public class CustomerService : ICustomerService
    {
        private readonly ILogger<CustomerService> logger;
        private readonly IServiceProvider serviceProvider;

        public CustomerService(ILogger<CustomerService> logger,
                               IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public async Task<Customer> GetCustomerAsync(long id)
        {
            using (var dataService = serviceProvider.GetService<ICustomerRepository>())
            {
                return await dataService.GetCustomerAsync(id);
            }
        }

        public async Task<IList<Customer>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request)
        {
            using (var dataService = serviceProvider.GetService<ICustomerRepository>())
            {
                var items = await dataService.GetCustomersAsync(skip, take, request);
                return items;
            }
        }

        public async Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            using (var dataService = serviceProvider.GetService<ICustomerRepository>())
            {
                return await dataService.GetCustomersCountAsync(request);
            }
        }

        public async Task<int> UpdateCustomerAsync(Customer customer)
        {
            using (var dataService = serviceProvider.GetService<ICustomerRepository>())
            {
                return await dataService.UpdateCustomerAsync(customer);
            }
        }

        public async Task<int> DeleteCustomerAsync(Customer customer)
        {
            using (var dataService = serviceProvider.GetService<ICustomerRepository>())
            {
                return await dataService.DeleteCustomersAsync(customer);
            }
        }

        public async Task<int> DeleteCustomerRangeAsync(int index, int length, DataRequest<Customer> request)
        {
            using (var dataService = serviceProvider.GetService<ICustomerRepository>())
            {
                var items = await dataService.GetCustomerKeysAsync(index, length, request);
                return await dataService.DeleteCustomersAsync(items.ToArray());
            }
        }

    }
}
