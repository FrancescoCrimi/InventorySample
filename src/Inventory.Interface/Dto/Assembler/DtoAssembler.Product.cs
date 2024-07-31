using Inventory.Domain.Model;
using System.Linq;

namespace Inventory.Interface.Dto
{
    public static partial class DtoAssembler
    {
        public static ProductDto DtoFromProduct(Product product, bool includeAllFields)
        {
            var model = new ProductDto()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Size = product.Size,
                Color = product.Color,
                ListPrice = product.ListPrice,
                DealerPrice = product.DealerPrice,
                Discount = product.Discount,
                DiscountStartDate = product.DiscountStartDate,
                DiscountEndDate = product.DiscountEndDate,
                StockUnits = product.StockUnits,
                SafetyStockLevel = product.SafetyStockLevel,
                CreatedOn = product.CreatedOn,
                LastModifiedOn = product.LastModifiedOn,
                Thumbnail = product.Thumbnail,

                CategoryId = product.CategoryId,
                TaxTypeId = product.TaxTypeId,
                Category = _lookupTable.Categories.FirstOrDefault(c => c.Id == product.CategoryId),
                TaxType = _lookupTable.TaxTypes.FirstOrDefault(t => t.Id == product.TaxTypeId)
            };
            if (includeAllFields)
            {
                model.Picture = product.Picture;
            }
            return model;
        }

        public static void UpdateProductFromDto(Product product, ProductDto dto)
        {
            product.CategoryId = dto.CategoryId;
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Size = dto.Size;
            product.Color = dto.Color;
            product.ListPrice = dto.ListPrice;
            product.DealerPrice = dto.DealerPrice;
            product.TaxTypeId = dto.TaxTypeId;
            product.Discount = dto.Discount;
            product.DiscountStartDate = dto.DiscountStartDate;
            product.DiscountEndDate = dto.DiscountEndDate;
            product.StockUnits = dto.StockUnits;
            product.SafetyStockLevel = dto.SafetyStockLevel;
            product.CreatedOn = dto.CreatedOn;
            product.LastModifiedOn = dto.LastModifiedOn;
            product.Picture = dto.Picture;
            product.Thumbnail = dto.Thumbnail;
        }

    }
}
