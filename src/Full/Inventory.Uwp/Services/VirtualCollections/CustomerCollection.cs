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

using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Library.Common;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class CustomerCollection : VirtualRangeCollection<CustomerDto>
    {
        private readonly ILogger _logger;
        private readonly CustomerServiceFacade _customerService;
        private DataRequest<Customer> _dataRequest;

        public CustomerCollection(ILogger<CustomerCollection> logger,
                                  CustomerServiceFacade customerService)
            : base(logger)
        {
            _logger = logger;
            _customerService = customerService;
        }

        public async Task LoadAsync(DataRequest<Customer> dataRequest)
        {
            _dataRequest = dataRequest;
            await LoadAsync();
        }

        protected override CustomerDto CreateDummyEntity()
        {
            return new CustomerDto() { FirstName = "Dummy Customer" };
        }

        protected override async Task<int> GetCountAsync()
        {
            int result = await _customerService.GetCustomersCountAsync(_dataRequest);
            return result;
        }

        protected override async Task<IList<CustomerDto>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            try
            {
                //Todo: fix cancellationToken
                var result = await _customerService.GetCustomersAsync(skip, take, _dataRequest, dispatcher);
                return result;
            }
            catch (System.Exception ex)
            {
                //LogException("CustomerCollection", "Fetch", ex);
                _logger.LogError(LogEvents.Fetch, ex, "Load Customer Error");
            }
            return null;
        }
    }
}
