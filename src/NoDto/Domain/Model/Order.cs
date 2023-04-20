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
    public partial class Order : Infrastructure.Common.ObservableObject<Order>
    {
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
        public string TrackingNumber
        {
            get; set;
        }
        public string ShipAddress
        {
            get; set;
        }
        public string ShipCity
        {
            get; set;
        }
        public string ShipRegion
        {
            get; set;
        }
        public string ShipPostalCode
        {
            get; set;
        }
        public string ShipPhone
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

        public long CustomerId
        {
            get; set;
        }
        public int? PaymentTypeId
        {
            get; set;
        }
        public long ShipCountryId
        {
            get; set;
        }
        public int? ShipperId
        {
            get; set;
        }
        public int StatusId
        {
            get; set;
        }


        public virtual Customer Customer
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
        public virtual Country ShipCountry
        {
            get; set;
        }
        public virtual OrderStatus Status
        {
            get; set;
        }
        public virtual ICollection<OrderItem> OrderItems
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
            OrderDate = source.OrderDate;
            ShippedDate = source.ShippedDate;
            DeliveredDate = source.DeliveredDate;
            TrackingNumber = source.TrackingNumber;
            ShipAddress = source.ShipAddress;
            ShipCity = source.ShipCity;
            ShipRegion = source.ShipRegion;
            ShipPostalCode = source.ShipPostalCode;
            ShipPhone = source.ShipPhone;
            LastModifiedOn = source.LastModifiedOn;
            SearchTerms = source.SearchTerms;

            CustomerId = source.CustomerId;
            PaymentTypeId = source.PaymentTypeId;
            ShipCountryId = source.ShipCountryId;
            ShipperId = source.ShipperId;
            StatusId = source.StatusId;
        }
    }
}
