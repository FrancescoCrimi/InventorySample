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

using Inventory.Domain.Model;
using System;

namespace Inventory.Interface.Dto
{
    public class ProductDto : ObservableDto<ProductDto>
    {
        private string _name;
        private string _description;
        private string _size;
        private string _color;
        private decimal _listPrice;
        private decimal _dealerPrice;
        private decimal _discount;
        private DateTimeOffset? _discountStartDate;
        private DateTimeOffset? _discountEndDate;
        private int _stockUnits;
        private int _safetyStockLevel;
        private DateTimeOffset _createdOn;
        private DateTimeOffset _lastModifiedOn;
        private byte[] _picture;
        private byte[] _thumbnail;
        private long _categoryId;
        private long _taxTypeId;
        private Category _category;
        private TaxType _taxType;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
        public string Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }
        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
        public decimal ListPrice
        {
            get => _listPrice;
            set => SetProperty(ref _listPrice, value);
        }
        public decimal DealerPrice
        {
            get => _dealerPrice;
            set => SetProperty(ref _dealerPrice, value);
        }
        public decimal Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value);
        }
        public DateTimeOffset? DiscountStartDate
        {
            get => _discountStartDate;
            set => SetProperty(ref _discountStartDate, value);
        }
        public DateTimeOffset? DiscountEndDate
        {
            get => _discountEndDate;
            set => SetProperty(ref _discountEndDate, value);
        }
        public int StockUnits
        {
            get => _stockUnits;
            set => SetProperty(ref _stockUnits, value);
        }
        public int SafetyStockLevel
        {
            get => _safetyStockLevel;
            set => SetProperty(ref _safetyStockLevel, value);
        }
        public DateTimeOffset CreatedOn
        {
            get => _createdOn;
            set => SetProperty(ref _createdOn, value);
        }
        public DateTimeOffset LastModifiedOn
        {
            get => _lastModifiedOn;
            set => SetProperty(ref _lastModifiedOn, value);
        }
        public byte[] Picture
        {
            get => _picture;
            set => SetProperty(ref _picture, value);
        }
        public byte[] Thumbnail
        {
            get => _thumbnail;
            set => SetProperty(ref _thumbnail, value);
        }



        public long CategoryId
        {
            get => _categoryId;
            set => SetProperty(ref _categoryId, value);
        }
        public long TaxTypeId
        {
            get => _taxTypeId;
            set => SetProperty(ref _taxTypeId, value);
        }
        public Category Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }
        public TaxType TaxType
        {
            get => _taxType;
            set => SetProperty(ref _taxType, value);
        }



        public string CategoryName => Category?.Name;


        public static ProductDto CreateEmpty() => new ProductDto { Id = -1, IsEmpty = true };

        public override void Merge(ProductDto source)
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
    }
}
