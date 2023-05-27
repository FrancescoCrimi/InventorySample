// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.Aggregates.CustomerAggregate;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public class CustomerService
    {
        private readonly ILogger _logger;
        private readonly ICustomerRepository _customerRepository;
        private static List<Country> _countries;

        public CustomerService(ILogger<CustomerService> logger,
                               ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
            Task.Run(async () => await GetCountryCodesAsync());
        }

        public IEnumerable<Country> CountryCodes => _countries;


        public Task<IList<Customer>> GetCustomersAsync(int index, int length, DataRequest<Customer> request)
        {
            return _customerRepository.GetCustomersAsync(index, length, request);
        }

        public Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            return _customerRepository.GetCustomersCountAsync(request);
        }

        public Task<Customer> GetCustomerAsync(long customerId)
        {
            return _customerRepository.GetCustomerAsync(customerId);
        }

        public Task UpdateCustomerAsync(Customer model)
        {
            return _customerRepository.UpdateCustomerAsync(model);
        }

        public Task DeleteCustomersAsync(Customer model)
        {
            return _customerRepository.DeleteCustomersAsync(model);
        }

        public async Task DeleteCustomersAsync(IEnumerable<Customer> models)
        {
            foreach (var model in models)
            {
                await _customerRepository.DeleteCustomersAsync(model);
            }
        }

        public async Task DeleteCustomerRangeAsync(int index, int length, DataRequest<Customer> request)
        {
            var items = await _customerRepository.GetCustomerKeysAsync(index, length, request);
            await _customerRepository.DeleteCustomersAsync(items.ToArray());
        }


        private async Task GetCountryCodesAsync()
        {
            if (_countries == null)
            {
                try
                {
                    _countries = await _customerRepository.GetCountryCodesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.LoadCountryCodes, ex, "Load CountryCodes");
                }
            }
        }
    }
}
