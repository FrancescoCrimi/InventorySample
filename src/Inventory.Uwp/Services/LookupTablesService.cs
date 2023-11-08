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

using Inventory.Domain.Repository;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public IList<CategoryDto> Categories { get; private set; }

        public IList<CountryDto> Countries { get; private set; }

        public IList<OrderStatusDto> OrderStatus { get; private set; }

        public IList<PaymentTypeDto> PaymentTypes { get; private set; }

        public IList<ShipperDto> Shippers { get; private set; }

        public IList<TaxTypeDto> TaxTypes { get; private set; }

        public async Task InitializeAsync()
        {
            Categories = await GetCategoriesAsync();
            Countries = await GetCountryCodesAsync();
            OrderStatus = await GetOrderStatusAsync();
            PaymentTypes = await GetPaymentTypesAsync();
            Shippers = await GetShippersAsync();
            TaxTypes = await GetTaxTypesAsync();
        }

        private async Task<IList<CategoryDto>> GetCategoriesAsync()
        {
            try
            {
                var items = await _repository.GetCategoriesAsync();
                return items.Select(r => new CategoryDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadCategories, ex, "Load Categories");
            }
            return new List<CategoryDto>();
        }

        private async Task<IList<CountryDto>> GetCountryCodesAsync()
        {
            try
            {
                var items = await _repository.GetCountryCodesAsync();
                return items.OrderBy(r => r.Name).Select(r => new CountryDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Name = r.Name
                })
                .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadCountryCodes, ex, "Load CountryCodes");
            }
            return new List<CountryDto>();
        }

        private async Task<IList<OrderStatusDto>> GetOrderStatusAsync()
        {
            try
            {
                var items = await _repository.GetOrderStatusAsync();
                return items.Select(r => new OrderStatusDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadOrderStatus, ex, "Load OrderStatus");
            }
            return new List<OrderStatusDto>();
        }

        private async Task<IList<PaymentTypeDto>> GetPaymentTypesAsync()
        {
            try
            {
                var items = await _repository.GetPaymentTypesAsync();
                return items.Select(r => new PaymentTypeDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadPaymentTypes, ex, "Load PaymentTypes");
            }
            return new List<PaymentTypeDto>();
        }

        private async Task<IList<ShipperDto>> GetShippersAsync()
        {
            try
            {
                var items = await _repository.GetShippersAsync();
                return items.Select(r => new ShipperDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Phone = r.Phone
                })
                .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadShippers, ex, "Load Shippers");
            }
            return new List<ShipperDto>();
        }

        private async Task<IList<TaxTypeDto>> GetTaxTypesAsync()
        {
            try
            {
                var items = await _repository.GetTaxTypesAsync();
                return items.Select(r => new TaxTypeDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Rate = r.Rate
                })
                .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadTaxTypes, ex, "Load TaxTypes");
            }
            return new List<TaxTypeDto>();
        }
    }
}
