﻿#region copyright
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
using Inventory.Uwp.Dto;
using Inventory.Uwp.Library.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class CustomerCollection : VirtualRangeCollection<CustomerDto>
    {
        private readonly CustomerServiceFacade customerService;
        private DataRequest<Customer> dataRequest;

        public CustomerCollection(CustomerServiceFacade customerService)
        {
            this.customerService = customerService;
        }

        public async Task LoadAsync(DataRequest<Customer> dataRequest)
        {
            this.dataRequest = dataRequest;
            await LoadAsync();
        }

        protected override CustomerDto CreateDummyEntity()
        {
            return new CustomerDto() { FirstName = "Dummy Customer" };
        }

        protected override async Task<int> GetCountAsync()
        {
            int result = await customerService.GetCustomersCountAsync(dataRequest);
            return result;
        }

        protected override async Task<IList<CustomerDto>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            //Todo: fix cancellationToken
            var result = await customerService.GetCustomersAsync(skip, take, dataRequest, dispatcher);
            return result;
        }
    }
}