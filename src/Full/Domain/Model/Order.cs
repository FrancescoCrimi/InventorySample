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
using Inventory.Infrastructure.DomainBase;

namespace Inventory.Domain.Model
{
    public class Order : Entity<Order>
    {
        #region property

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

        #endregion


        #region relation

        public long CustomerId
        {
            get; set;
        }
        public long? PaymentTypeId
        {
            get; set;
        }
        public long ShipCountryId
        {
            get; set;
        }
        public long? ShipperId
        {
            get; set;
        }
        public long StatusId
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

        #endregion


        #region not mapped

        [NotMapped]
        public bool CanEditPayment => StatusId > 0;
        [NotMapped]
        public bool CanEditShipping => StatusId > 1;
        [NotMapped]
        public bool CanEditDelivery => StatusId > 2;
        [NotMapped]
        public string StatusDesc => Status == null ? string.Empty : Status.Name;
        [NotMapped]
        public string PaymentTypeDesc => PaymentType == null ? string.Empty : PaymentType.Name;
        [NotMapped]
        public string ShipViaDesc => Shipper == null ? string.Empty : Shipper.Name;
        [NotMapped]
        public string ShipCountryName => ShipCountry == null ? string.Empty : ShipCountry.Name;

        #endregion


        #region method

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
                Customer = customer,
            };
        }

        public string BuildSearchTerms() => $"{Id} {CustomerId} {ShipCity} {ShipRegion}".ToLower();

        #endregion
    }
}
