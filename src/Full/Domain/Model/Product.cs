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

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Inventory.Infrastructure.DomainBase;

namespace Inventory.Domain.Model
{
    public class Product : Entity<Product>
    {
        #region property

        public string Name
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }
        public string Size
        {
            get; set;
        }
        public string Color
        {
            get; set;
        }
        public decimal ListPrice
        {
            get; set;
        }
        public decimal DealerPrice
        {
            get; set;
        }
        public decimal Discount
        {
            get; set;
        }
        public DateTimeOffset? DiscountStartDate
        {
            get; set;
        }
        public DateTimeOffset? DiscountEndDate
        {
            get; set;
        }
        public int StockUnits
        {
            get; set;
        }
        public int SafetyStockLevel
        {
            get; set;
        }
        public DateTimeOffset CreatedOn
        {
            get; set;
        }
        public DateTimeOffset LastModifiedOn
        {
            get; set;
        }
        public string SearchTerms
        {
            get; set;
        }
        public byte[] Picture
        {
            get; set;
        }
        public byte[] Thumbnail
        {
            get; set;
        }

        #endregion


        #region relation

        public long CategoryId
        {
            get; set;
        }

        public long TaxTypeId
        {
            get; set;
        }

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


        #region method

        public string BuildSearchTerms() => $"{Id} {Name} {Color}".ToLower();

        #endregion
    }
}
