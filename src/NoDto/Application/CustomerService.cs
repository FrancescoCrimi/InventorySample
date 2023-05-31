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
        }

        public IEnumerable<Country> Countries
        {
            get
            {
                if (_countries == null)
                {
                    try
                    {
                        _countries = _customerRepository.GetCountriesAsync().Result;
                    }
                    catch (Exception ex)
                    {
                        _countries = new List<Country>();
                        _logger.LogError(LogEvents.LoadCountryCodes, ex, "Load CountryCodes");
                    }
                }
                return _countries;
            }
        }

        public async Task<IList<Customer>> GetCustomersAsync(int index, int length, DataRequest<Customer> request)
        {
            return await _customerRepository.GetCustomersAsync(index, length, request);
        }

        public async Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            return await _customerRepository.GetCustomersCountAsync(request);
        }

        public async Task<Customer> GetCustomerAsync(long customerId)
        {
            return await _customerRepository.GetCustomerAsync(customerId);
        }

        public async Task UpdateCustomerAsync(Customer model)
        {
            await _customerRepository.UpdateCustomerAsync(model);
        }

        public async Task DeleteCustomersAsync(Customer model)
        {
            await _customerRepository.DeleteCustomersAsync(model);
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
    }
}
