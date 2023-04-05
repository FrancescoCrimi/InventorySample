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

using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Library.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class CustomerCollection : VirtualRangeCollection<Customer>
    {
        private readonly ICustomerService _customerService;
        private DataRequest<Customer> dataRequest;

        public CustomerCollection(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task LoadAsync(DataRequest<Customer> dataRequest)
        {
            this.dataRequest = dataRequest;
            await LoadAsync();
        }

        protected override Customer CreateDummyEntity()
        {
            return new Customer() { FirstName = "Dummy Customer" };
        }

        protected async override Task<int> GetCountAsync()
        {
            var result = await _customerService.GetCustomersCountAsync(dataRequest);
            return result;
        }

        protected async override Task<IList<Customer>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            //Todo: fix cancellationToken
            var result = await _customerService.GetCustomersAsync(skip, take, dataRequest);
            return result;
        }
    }
}
