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
using Inventory.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public class LookupTablesService
    {
        private readonly ILogger _logger;
        private readonly ILookupTableRepository _repository;

        public LookupTablesService(ILogger<LookupTablesService> logger,
                                   ILookupTableRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public IList<Category> Categories { get; private set; }

        public IList<Country> Countries { get; private set; }

        public IList<OrderStatus> OrderStatus { get; private set; }

        public IList<PaymentType> PaymentTypes { get; private set; }

        public IList<Shipper> Shippers { get; private set; }

        public IList<TaxType> TaxTypes { get; private set; }

        public async Task InitializeAsync()
        {
            Categories = await GetCategoriesAsync();
            Countries = await GetCountryCodesAsync();
            OrderStatus = await GetOrderStatusAsync();
            PaymentTypes = await GetPaymentTypesAsync();
            Shippers = await GetShippersAsync();
            TaxTypes = await GetTaxTypesAsync();
        }

        private async Task<IList<Category>> GetCategoriesAsync()
        {
            try
            {
                var items = await _repository.GetCategoriesAsync();
                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadCategories, ex, "Load Categories");
                throw new Exception("Load Categories", ex);
            }
        }

        private async Task<IList<Country>> GetCountryCodesAsync()
        {
            try
            {
                var items = await _repository.GetCountryCodesAsync();
                return items.OrderBy(r => r.Name).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadCountryCodes, ex, "Load CountryCodes");
                throw new Exception("Load CountryCodes", ex);
            }
        }

        private async Task<IList<OrderStatus>> GetOrderStatusAsync()
        {
            try
            {
                var items = await _repository.GetOrderStatusAsync();
                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadOrderStatus, ex, "Load OrderStatus");
                throw new Exception("Load OrderStatus", ex);
            }
        }

        private async Task<IList<PaymentType>> GetPaymentTypesAsync()
        {
            try
            {
                var items = await _repository.GetPaymentTypesAsync();
                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadPaymentTypes, ex, "Load PaymentTypes");
                throw new Exception("Load PaymentTypes", ex) ;
            }
        }

        private async Task<IList<Shipper>> GetShippersAsync()
        {
            try
            {
                var items = await _repository.GetShippersAsync();
            return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadShippers, ex, "Load Shippers");
                throw new Exception("Load Shippers", ex);
            }
        }

        private async Task<IList<TaxType>> GetTaxTypesAsync()
        {
            try
            {
                var items = await _repository.GetTaxTypesAsync();
                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadTaxTypes, ex, "Load TaxTypes");
                throw new Exception("Load TaxTypes", ex);
            }
        }
    }
}
