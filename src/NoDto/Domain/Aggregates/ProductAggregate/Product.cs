// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Domain.Aggregates.ProductAggregate
{
    public class Product : Infrastructure.Common.ObservableObject<Product>, IEquatable<Product>
    {
        #region fields

        private string name;
        private string description;
        private string size;
        private string color;
        private decimal listPrice;
        private decimal dealerPrice;
        private decimal discount;
        private DateTimeOffset? discountStartDate;
        private DateTimeOffset? discountEndDate;
        private int stockUnits;
        private int safetyStockLevel;
        private DateTimeOffset createdOn;
        private DateTimeOffset lastModifiedOn;
        private string searchTerms;
        private byte[] picture;
        private byte[] thumbnail;
        private long categoryId;
        private long taxTypeId;

        #endregion


        #region Property

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }
        public string Size
        {
            get => size;
            set => SetProperty(ref size, value);
        }
        public string Color
        {
            get => color;
            set => SetProperty(ref color, value);
        }
        public decimal ListPrice
        {
            get => listPrice;
            set => SetProperty(ref listPrice, value);
        }
        public decimal DealerPrice
        {
            get => dealerPrice;
            set => SetProperty(ref dealerPrice, value);
        }
        public decimal Discount
        {
            get => discount;
            set => SetProperty(ref discount, value);
        }
        public DateTimeOffset? DiscountStartDate
        {
            get => discountStartDate;
            set => SetProperty(ref discountStartDate, value);
        }
        public DateTimeOffset? DiscountEndDate
        {
            get => discountEndDate;
            set => SetProperty(ref discountEndDate, value);
        }
        public int StockUnits
        {
            get => stockUnits;
            set => SetProperty(ref stockUnits, value);
        }
        public int SafetyStockLevel
        {
            get => safetyStockLevel;
            set => SetProperty(ref safetyStockLevel, value);
        }
        public DateTimeOffset CreatedOn
        {
            get => createdOn;
            set => SetProperty(ref createdOn, value);
        }
        public DateTimeOffset LastModifiedOn
        {
            get => lastModifiedOn;
            set => SetProperty(ref lastModifiedOn, value);
        }
        public string SearchTerms
        {
            get => searchTerms;
            set => SetProperty(ref searchTerms, value);
        }
        public byte[] Picture
        {
            get => picture;
            set => SetProperty(ref picture, value);
        }
        public byte[] Thumbnail
        {
            get => thumbnail;
            set => SetProperty(ref thumbnail, value);
        }

        public long CategoryId
        {
            get => categoryId;
            set => SetProperty(ref categoryId, value);
        }
        public long TaxTypeId
        {
            get => taxTypeId;
            set => SetProperty(ref taxTypeId, value);
        }

        #endregion


        #region relation

        public virtual Category Category
        {
            get; set;
        }
        public virtual TaxType TaxType
        {
            get; set;
        }

        #endregion


        #region not mapped

        [NotMapped]
        public string CategoryName => Category?.Name;

        #endregion


        #region Method

        public string BuildSearchTerms() => $"{Id} {Name} {Color}".ToLower();

        #endregion


        #region Equals

        public override bool Equals(object obj)
        {
            return Equals(obj as Product);
        }

        public bool Equals(Product other)
        {
            return !(other is null) &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(Product left, Product right)
        {
            return EqualityComparer<Product>.Default.Equals(left, right);
        }

        public static bool operator !=(Product left, Product right)
        {
            return !(left == right);
        }

        #endregion
    }
}
