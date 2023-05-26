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
using Inventory.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            Task.Run(async() => await GetCountryCodesAsync());
        }

        public IEnumerable<Country> CountryCodes => _countries;

        private async Task GetCountryCodesAsync()
        {
            if (_countries == null)
            {
                try
                {
                    _countries =  await _customerRepository.GetCountryCodesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.LoadCountryCodes, ex, "Load CountryCodes");
                }
            }
        }
    }
}
