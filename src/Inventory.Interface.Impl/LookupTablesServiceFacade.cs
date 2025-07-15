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

using Inventory.Application;
using Inventory.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Interface.Impl
{
    public class LookupTablesServiceFacade : ILookupTablesServiceFacade
    {
        private readonly ILogger _logger;
        private readonly LookupTablesService _lookupTablesService;

        public LookupTablesServiceFacade(ILogger<LookupTablesServiceFacade> logger,
                                   LookupTablesService lookupTablesService)
        {
            _logger = logger;
            _lookupTablesService = lookupTablesService;
        }

        public IList<Category> Categories { get; private set; }

        public IList<Country> Countries { get; private set; }

        public IList<OrderStatus> OrderStatus { get; private set; }

        public IList<PaymentType> PaymentTypes { get; private set; }

        public IList<Shipper> Shippers { get; private set; }

        public IList<TaxType> TaxTypes { get; private set; }

        public async Task InitializeAsync()
        {
            await _lookupTablesService.InitializeAsync();

            Categories = _lookupTablesService.Categories;
            Countries = _lookupTablesService.Countries;
            OrderStatus = _lookupTablesService.OrderStatus;
            PaymentTypes = _lookupTablesService.PaymentTypes;
            Shippers = _lookupTablesService.Shippers;
            TaxTypes = _lookupTablesService.TaxTypes;

            await Task.CompletedTask;
        }
    }
}
