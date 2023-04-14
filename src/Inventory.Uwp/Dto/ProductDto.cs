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

using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Uwp.Services;
using System;

namespace Inventory.Uwp.Dto
{
    public class ProductDto : ObservableDto
    {
        public static ProductDto CreateEmpty() => new ProductDto { Id = -1, IsEmpty = true };

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal ListPrice { get; set; }
        public decimal DealerPrice { get; set; }
        public decimal Discount { get; set; }
        public DateTimeOffset? DiscountStartDate { get; set; }
        public DateTimeOffset? DiscountEndDate { get; set; }
        public int StockUnits { get; set; }
        public int SafetyStockLevel { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }
        public byte[] Picture { get; set; }
        public byte[] Thumbnail { get; set; }

        public int CategoryId { get; set; }
        public int TaxTypeId { get; set; }
        public CategoryDto Category { get; set; }
        public TaxTypeDto TaxType { get; set; }

        public bool IsNew => Id <= 0;
        public string CategoryName => Ioc.Default.GetRequiredService<LookupTableServiceFacade>().GetCategory(CategoryId);

        public override void Merge(ObservableDto source)
        {
            if (source is ProductDto model)
            {
                Merge(model);
            }
        }

        public void Merge(ProductDto source)
        {
            if (source != null)
            {
                Id = source.Id;
                Name = source.Name;
                Description = source.Description;
                Size = source.Size;
                Color = source.Color;
                ListPrice = source.ListPrice;
                DealerPrice = source.DealerPrice;
                Discount = source.Discount;
                DiscountStartDate = source.DiscountStartDate;
                DiscountEndDate = source.DiscountEndDate;
                StockUnits = source.StockUnits;
                SafetyStockLevel = source.SafetyStockLevel;
                CreatedOn = source.CreatedOn;
                LastModifiedOn = source.LastModifiedOn;
                Picture = source.Picture;
                Thumbnail = source.Thumbnail;

                CategoryId = source.CategoryId;
                TaxTypeId = source.TaxTypeId;
                Category = source.Category;
                TaxType = source.TaxType;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
