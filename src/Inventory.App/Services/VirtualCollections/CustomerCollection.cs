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

using CiccioSoft.Inventory.Data;
using CiccioSoft.Inventory.Data.Models;
using CiccioSoft.Inventory.Data.Services;
using CiccioSoft.Inventory.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services
{
    public class CustomerCollection : VirtualCollection<CustomerModel>
    {
        private DataRequest<Customer> _dataRequest = null;
        private readonly ILogger<CustomerCollection> logger = Ioc.Default.GetService<ILogger<CustomerCollection>>();

        public CustomerCollection(ICustomerService customerService)
            : base()
        {
            CustomerService = customerService;
        }

        public ICustomerService CustomerService { get; }

        private CustomerModel _defaultItem = CustomerModel.CreateEmpty();
        protected override CustomerModel DefaultItem => _defaultItem;

        public async Task LoadAsync(DataRequest<Customer> dataRequest)
        {
            try
            {
                _dataRequest = dataRequest;
                Count = await CustomerService.GetCustomersCountAsync(_dataRequest);
                Ranges[0] = await CustomerService.GetCustomersAsync(0, RangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                Count = 0;
                throw ex;
            }
        }

        protected override async Task<IList<CustomerModel>> FetchDataAsync(int rangeIndex, int rangeSize)
        {
            try
            {
                return await CustomerService.GetCustomersAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fetch");
            }
            return null;
        }
    }
}
