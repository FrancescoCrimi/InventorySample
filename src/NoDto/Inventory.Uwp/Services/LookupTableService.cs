using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.Services
{
    public class LookupTableService /*: ILookupTableService*/
    {
        private readonly ILogger<LookupTableService> _logger;
        private readonly ILookupTableRepository _repository;

        public LookupTableService(ILogger<LookupTableService> logger,
                                  ILookupTableRepository lookupTableRepository)
        {
            _logger = logger;
            _repository = lookupTableRepository;
            Task.Run(async () => await InitializeAsync());
        }

        public IList<Category> Categories
        {
            get; private set;
        }

        public IList<Country> CountryCodes
        {
            get; private set;
        }

        public IList<OrderStatus> OrderStatus
        {
            get; private set;
        }

        public IList<PaymentType> PaymentTypes
        {
            get; private set;
        }

        public IList<Shipper> Shippers
        {
            get; private set;
        }

        public IList<TaxType> TaxTypes
        {
            get; private set;
        }

        public string GetCategory(int id)
        {
            return Categories.Where(r => r.Id == id).Select(r => r.Name).FirstOrDefault();
        }

        public string GetCountry(string id)
        {
            return CountryCodes.Where(r => r.Code == id).Select(r => r.Name).FirstOrDefault();
        }

        public string GetOrderStatus(int id)
        {
            return OrderStatus.Where(r => r.Id == id).Select(r => r.Name).FirstOrDefault();
        }

        public string GetPaymentType(int? id)
        {
            return id == null ? "" : PaymentTypes.Where(r => r.Id == id).Select(r => r.Name).FirstOrDefault();
        }

        public string GetShipper(int? id)
        {
            return id == null ? "" : Shippers.Where(r => r.Id == id).Select(r => r.Name).FirstOrDefault();
        }

        public string GetTaxDesc(int id)
        {
            return TaxTypes.Where(r => r.Id == id).Select(r => $"{r.Rate} %").FirstOrDefault();
        }

        public decimal GetTaxRate(int id)
        {
            return TaxTypes.Where(r => r.Id == id).Select(r => r.Rate).FirstOrDefault();
        }

        public async Task InitializeAsync()
        {
            Categories = await GetCategoriesAsync();
            CountryCodes = await GetCountryCodesAsync();
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
                return items.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load Categories");
            }
            return new List<Category>();
        }

        private async Task<IList<Country>> GetCountryCodesAsync()
        {
            try
            {
                var items = await _repository.GetCountryCodesAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load CountryCodes");
            }
            return new List<Country>();
        }

        private async Task<IList<OrderStatus>> GetOrderStatusAsync()
        {
            try
            {
                var items = await _repository.GetOrderStatusAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load OrderStatus");
            }
            return new List<OrderStatus>();
        }

        private async Task<IList<PaymentType>> GetPaymentTypesAsync()
        {
            try
            {
                var items = await _repository.GetPaymentTypesAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load PaymentTypes");
            }
            return new List<PaymentType>();
        }

        private async Task<IList<Shipper>> GetShippersAsync()
        {
            try
            {
                var items = await _repository.GetShippersAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load Shippers");
            }
            return new List<Shipper>();
        }

        private async Task<IList<TaxType>> GetTaxTypesAsync()
        {
            try
            {
                var items = await _repository.GetTaxTypesAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load TaxTypes");
            }
            return new List<TaxType>();
        }
    }
}
