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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Library.Common;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class CustomerCollection : VirtualRangeCollection<Customer>
    {
        private readonly ILogger _logger;
        private readonly ICustomerRepository _customerRepository;
        private DataRequest<Customer> _request;

        public CustomerCollection(ILogger<CustomerCollection> logger,
                                  ICustomerRepository customerRepository)
            : base(logger)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        // TODO: fix here request
        public async Task LoadAsync(DataRequest<Customer> request)
        {
            _request = request;
            await LoadAsync();
        }

        protected override Customer CreateDummyEntity()
        {
            return new Customer() { FirstName = "Dummy Customer" };
        }

        protected async override Task<int> GetCountAsync()
        {
            var result = await _customerRepository.GetCustomersCountAsync(_request);
            return result;
        }

        protected async override Task<IList<Customer>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            try
            {
                //Todo: fix cancellationToken
                var result = await _customerRepository.GetCustomersAsync(skip, take, _request);
                return result;
            }
            catch (Exception ex)
            {
                //LogException("CustomerCollection", "Fetch", ex);
                _logger.LogError(LogEvents.Fetch, ex, "Load Customer Error");
            }
            return null;
        }
    }
}
