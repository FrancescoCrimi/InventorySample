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
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Domain.Model
{
    public class Order : Infrastructure.Common.ObservableObject<Order>, IEquatable<Order>
    {
        private DateTimeOffset orderDate;
        private DateTimeOffset? shippedDate;
        private DateTimeOffset? deliveredDate;
        private string trackingNumber;
        private string shipAddress;
        private string shipCity;
        private string shipRegion;
        private string shipPostalCode;
        private string shipPhone;
        private DateTimeOffset lastModifiedOn;
        private string searchTerms;
        private long customerId;
        private long? paymentTypeId;
        private long shipCountryId;
        private long? shipperId;
        private long statusId;


        #region property

        public DateTimeOffset OrderDate
        {
            get => orderDate;
            set => SetProperty(ref orderDate, value);
        }
        public DateTimeOffset? ShippedDate
        {
            get => shippedDate;
            set => SetProperty(ref shippedDate, value);
        }
        public DateTimeOffset? DeliveredDate
        {
            get => deliveredDate;
            set => SetProperty(ref deliveredDate, value);
        }
        public string TrackingNumber
        {
            get => trackingNumber;
            set => SetProperty(ref trackingNumber, value);
        }
        public string ShipAddress
        {
            get => shipAddress;
            set => SetProperty(ref shipAddress, value);
        }
        public string ShipCity
        {
            get => shipCity;
            set => SetProperty(ref shipCity, value);
        }
        public string ShipRegion
        {
            get => shipRegion;
            set => SetProperty(ref shipRegion, value);
        }
        public string ShipPostalCode
        {
            get => shipPostalCode;
            set => SetProperty(ref shipPostalCode, value);
        }
        public string ShipPhone
        {
            get => shipPhone;
            set => SetProperty(ref shipPhone, value);
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

        public long CustomerId
        {
            get => customerId;
            set => SetProperty(ref customerId, value);
        }
        public long? PaymentTypeId
        {
            get => paymentTypeId;
            set => SetProperty(ref paymentTypeId, value);
        }
        public long ShipCountryId
        {
            get => shipCountryId;
            set => SetProperty(ref shipCountryId, value);
        }
        public long? ShipperId
        {
            get => shipperId;
            set => SetProperty(ref shipperId, value);
        }
        public long StatusId
        {
            get => statusId;
            set => SetProperty(ref statusId, value);
        }

        #endregion


        #region relation

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

        #endregion


        #region not mapped

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

        #endregion


        #region public method

        public string BuildSearchTerms() => $"{Id} {CustomerId} {ShipCity} {ShipRegion}".ToLower();

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

        public static Order CreateNewOrder(Customer customer)
        {
            return new Order
            {
                CustomerId = customer.Id,
                OrderDate = DateTime.UtcNow,
                StatusId = 0,
                ShipAddress = customer.AddressLine1,
                ShipCity = customer.City,
                ShipRegion = customer.Region,
                ShipCountryId = customer.CountryId,
                ShipPostalCode = customer.PostalCode,
                Customer = customer
            };
        }

        #endregion


        #region equals

        public override bool Equals(object obj)
        {
            return Equals(obj as Order);
        }

        public bool Equals(Order other)
        {
            return !(other is null) &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(Order left, Order right)
        {
            return EqualityComparer<Order>.Default.Equals(left, right);
        }

        public static bool operator !=(Order left, Order right)
        {
            return !(left == right);
        }

        #endregion
    }
}
