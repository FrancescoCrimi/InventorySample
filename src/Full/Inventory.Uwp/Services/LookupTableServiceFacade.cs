using Inventory.Domain.Repository;
using Inventory.Uwp.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services
{
    public class LookupTableServiceFacade /*: ILookupTableService*/
    {
        private readonly ILogger<LookupTableServiceFacade> logger;
        private readonly ILookupTableRepository lookupTableRepository;

        public LookupTableServiceFacade(ILogger<LookupTableServiceFacade> logger,
            ILookupTableRepository lookupTableRepository)
        {
            this.logger = logger;
            this.lookupTableRepository = lookupTableRepository;
        }

        public IList<CategoryDto> Categories { get; private set; }

        public IList<CountryDto> Countries { get; private set; }

        public IList<OrderStatusDto> OrderStatus { get; private set; }

        public IList<PaymentTypeDto> PaymentTypes { get; private set; }

        public IList<ShipperDto> Shippers { get; private set; }

        public IList<TaxTypeDto> TaxTypes { get; private set; }

        public string GetCategory(int id)
        {
            return Categories.Where(r => r.Id == id).Select(r => r.Name).FirstOrDefault();
        }

        public string GetCountry(long id)
        {
            return Countries.Where(r => r.Id == id).Select(r => r.Name).FirstOrDefault();
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
                //using (var dataService = serviceProvider.GetService<ILookupTableRepository>())
                //{
                var items = await lookupTableRepository.GetCategoriesAsync();
                return items.Select(r => new CategoryDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToList();
                //}
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Categories");
            }
            return new List<CategoryDto>();
        }

        private async Task<IList<CountryDto>> GetCountryCodesAsync()
        {
            try
            {
                //using (var dataService = serviceProvider.GetService<ILookupTableRepository>())
                //{
                var items = await lookupTableRepository.GetCountryCodesAsync();
                return items.OrderBy(r => r.Name).Select(r => new CountryDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Name = r.Name
                })
                .ToList();
                //}
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load CountryCodes");
            }
            return new List<CountryDto>();
        }

        private async Task<IList<OrderStatusDto>> GetOrderStatusAsync()
        {
            try
            {
                //using (var dataService = serviceProvider.GetService<ILookupTableRepository>())
                //{
                var items = await lookupTableRepository.GetOrderStatusAsync();
                return items.Select(r => new OrderStatusDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToList();
                //}
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load OrderStatus");
            }
            return new List<OrderStatusDto>();
        }

        private async Task<IList<PaymentTypeDto>> GetPaymentTypesAsync()
        {
            try
            {
                //using (var dataService = serviceProvider.GetService<ILookupTableRepository>())
                //{
                var items = await lookupTableRepository.GetPaymentTypesAsync();
                return items.Select(r => new PaymentTypeDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToList();
                //}
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load PaymentTypes");
            }
            return new List<PaymentTypeDto>();
        }

        private async Task<IList<ShipperDto>> GetShippersAsync()
        {
            try
            {
                //using (var dataService = serviceProvider.GetService<ILookupTableRepository>())
                //{
                var items = await lookupTableRepository.GetShippersAsync();
                return items.Select(r => new ShipperDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Phone = r.Phone
                })
                .ToList();
                //}
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Shippers");
            }
            return new List<ShipperDto>();
        }

        private async Task<IList<TaxTypeDto>> GetTaxTypesAsync()
        {
            try
            {
                //using (var dataService = serviceProvider.GetService<ILookupTableRepository>())
                //{
                var items = await lookupTableRepository.GetTaxTypesAsync();
                return items.Select(r => new TaxTypeDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Rate = r.Rate
                })
                .ToList();
                //}
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load TaxTypes");
            }
            return new List<TaxTypeDto>();
        }
    }
}
