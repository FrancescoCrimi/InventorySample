// Copyright (c) Microsoft. All rights reserved.
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
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services
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
            Categories = new ObservableCollection<Category>();
            CountryCodes = new ObservableCollection<Country>();
            OrderStatus = new ObservableCollection<OrderStatus>();
            PaymentTypes = new ObservableCollection<PaymentType>();
            Shippers = new ObservableCollection<Shipper>();
            TaxTypes = new ObservableCollection<TaxType>();
        }

        public ObservableCollection<Category> Categories
        {
            get; private set;
        }

        public ObservableCollection<Country> CountryCodes
        {
            get; private set;
        }

        public ObservableCollection<OrderStatus> OrderStatus
        {
            get; private set;
        }

        public ObservableCollection<PaymentType> PaymentTypes
        {
            get; private set;
        }

        public ObservableCollection<Shipper> Shippers
        {
            get; private set;
        }

        public ObservableCollection<TaxType> TaxTypes
        {
            get; private set;
        }


        public async Task InitializeAsync()
        {
            await GetCategoriesAsync();
            await GetCountryCodesAsync();
            await GetOrderStatusAsync();
            await GetPaymentTypesAsync();
            await GetShippersAsync();
            await GetTaxTypesAsync();
        }

        private async Task GetCategoriesAsync()
        {
            try
            {
                var items = await _repository.GetCategoriesAsync();
                Categories.Clear();
                foreach (var item in items)
                {
                    Categories.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadCategories, ex, "Load Categories");
            }
        }

        private async Task GetCountryCodesAsync()
        {
            try
            {
                var items = await _repository.GetCountryCodesAsync();
                CountryCodes.Clear();
                foreach (var item in items)
                {
                    CountryCodes.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadCountryCodes, ex, "Load CountryCodes");
            }
        }

        private async Task GetOrderStatusAsync()
        {
            try
            {
                var items = await _repository.GetOrderStatusAsync();
                OrderStatus.Clear();
                foreach (var item in items)
                {
                    OrderStatus.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadOrderStatus, ex, "Load OrderStatus");
            }
        }

        private async Task GetPaymentTypesAsync()
        {
            try
            {
                var items = await _repository.GetPaymentTypesAsync();
                PaymentTypes.Clear();
                foreach (var item in items)
                {
                    PaymentTypes.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadPaymentTypes, ex, "Load PaymentTypes");
            }
        }

        private async Task GetShippersAsync()
        {
            try
            {
                var items = await _repository.GetShippersAsync();
                Shippers.Clear();
                foreach (var item in items)
                {
                    Shippers.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadShippers, ex, "Load Shippers");
            }
        }

        private async Task GetTaxTypesAsync()
        {
            try
            {
                var items = await _repository.GetTaxTypesAsync();
                TaxTypes.Clear();
                foreach (var item in items)
                {
                    TaxTypes.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadTaxTypes, ex, "Load TaxTypes");
            }
        }
    }
}
