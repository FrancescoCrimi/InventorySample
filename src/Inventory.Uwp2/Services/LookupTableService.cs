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
        private readonly ILogger<LookupTableService> logger;
        private readonly ILookupTableRepository lookupTableRepository;

        public LookupTableService(ILogger<LookupTableService> logger,
            ILookupTableRepository lookupTableRepository)
        {
            this.logger = logger;
            this.lookupTableRepository = lookupTableRepository;
        }

        public IList<Category> Categories
        {
            get; private set;
        }

        public IList<CountryCode> CountryCodes
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
            return Categories.Where(r => r.CategoryID == id).Select(r => r.Name).FirstOrDefault();
        }

        public string GetCountry(string id)
        {
            return CountryCodes.Where(r => r.CountryCodeID == id).Select(r => r.Name).FirstOrDefault();
        }

        public string GetOrderStatus(int id)
        {
            return OrderStatus.Where(r => r.Status == id).Select(r => r.Name).FirstOrDefault();
        }

        public string GetPaymentType(int? id)
        {
            return id == null ? "" : PaymentTypes.Where(r => r.PaymentTypeID == id).Select(r => r.Name).FirstOrDefault();
        }

        public string GetShipper(int? id)
        {
            return id == null ? "" : Shippers.Where(r => r.ShipperID == id).Select(r => r.Name).FirstOrDefault();
        }

        public string GetTaxDesc(int id)
        {
            return TaxTypes.Where(r => r.TaxTypeID == id).Select(r => $"{r.Rate} %").FirstOrDefault();
        }

        public decimal GetTaxRate(int id)
        {
            return TaxTypes.Where(r => r.TaxTypeID == id).Select(r => r.Rate).FirstOrDefault();
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
                var items = await lookupTableRepository.GetCategoriesAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Categories");
            }
            return new List<Category>();
        }

        private async Task<IList<CountryCode>> GetCountryCodesAsync()
        {
            try
            {
                var items = await lookupTableRepository.GetCountryCodesAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load CountryCodes");
            }
            return new List<CountryCode>();
        }

        private async Task<IList<OrderStatus>> GetOrderStatusAsync()
        {
            try
            {
                var items = await lookupTableRepository.GetOrderStatusAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load OrderStatus");
            }
            return new List<OrderStatus>();
        }

        private async Task<IList<PaymentType>> GetPaymentTypesAsync()
        {
            try
            {
                var items = await lookupTableRepository.GetPaymentTypesAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load PaymentTypes");
            }
            return new List<PaymentType>();
        }

        private async Task<IList<Shipper>> GetShippersAsync()
        {
            try
            {
                var items = await lookupTableRepository.GetShippersAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Shippers");
            }
            return new List<Shipper>();
        }

        private async Task<IList<TaxType>> GetTaxTypesAsync()
        {
            try
            {
                var items = await lookupTableRepository.GetTaxTypesAsync();
                return items.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load TaxTypes");
            }
            return new List<TaxType>();
        }
    }
}
