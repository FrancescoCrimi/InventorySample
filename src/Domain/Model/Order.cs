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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inventory.Domain.Common;

namespace Inventory.Domain.Model
{
    [Table("Orders")]
    public partial class Order : ObservableObject<Order>
    {
        [Required]
        public long CustomerId
        {
            get; set;
        }

        [Required]
        public DateTimeOffset OrderDate
        {
            get; set;
        }
        public DateTimeOffset? ShippedDate
        {
            get; set;
        }
        public DateTimeOffset? DeliveredDate
        {
            get; set;
        }

        [Required]
        public int StatusId
        {
            get; set;
        }

        public int? PaymentTypeId
        {
            get; set;
        }

        [MaxLength(50)]
        public string TrackingNumber
        {
            get; set;
        }

        public int? ShipperId
        {
            get; set;
        }

        [MaxLength(120)]
        public string ShipAddress
        {
            get; set;
        }

        [MaxLength(30)]
        public string ShipCity
        {
            get; set;
        }

        [MaxLength(50)]
        public string ShipRegion
        {
            get; set;
        }

        [MaxLength(2)]
        public string ShipCountryCode
        {
            get; set;
        }

        [MaxLength(15)]
        public string ShipPostalCode
        {
            get; set;
        }

        [MaxLength(20)]
        public string ShipPhone
        {
            get; set;
        }

        [Required]
        public DateTimeOffset LastModifiedOn
        {
            get; set;
        }

        public string SearchTerms
        {
            get; set;
        }



        public virtual Customer Customer
        {
            get; set;
        }

        public virtual ICollection<OrderItem> OrderItems
        {
            get; set;
        }

        public virtual OrderStatus Status
        {
            get; set;
        }

        public virtual PaymentType PaymentType
        {
            get; set;
        }

        public virtual Shipper Shipper
        {
            get; set;
        }



        public string BuildSearchTerms() => $"{Id} {CustomerId} {ShipCity} {ShipRegion}".ToLower();



        [NotMapped]
        public bool CanEditPayment => StatusId > 0;
        [NotMapped]
        public bool CanEditShipping => StatusId > 1;
        [NotMapped]
        public bool CanEditDelivery => StatusId > 2;

        [NotMapped]
        public string StatusDesc => "Fake Status Desc";
        [NotMapped]
        public string PaymentTypeDesc => "Fake Payment Type Desc";
        [NotMapped]
        public string ShipViaDesc => "Fake Ship Via Desc";
        [NotMapped]
        public string ShipCountryName => "Fake Ship Country Name";

        public override void Merge(Order source)
        {
            //throw new NotImplementedException();
        }
    }
}
