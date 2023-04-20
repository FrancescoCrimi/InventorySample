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

namespace Inventory.Uwp.Dto
{
    public class OrderDto : ObservableDto, IEquatable<OrderDto>
    {
        private DateTimeOffset _orderDate;
        private DateTimeOffset? _shippedDate;
        private DateTimeOffset? _deliveredDate;
        private int _status;

        public static OrderDto CreateEmpty() => new OrderDto { Id = -1, CustomerId = -1, IsEmpty = true };

        public long Id { get; set; }
        public DateTimeOffset OrderDate
        {
            get => _orderDate;
            set => SetProperty(ref _orderDate, value);
        }
        public DateTimeOffset? ShippedDate
        {
            get => _shippedDate;
            set => SetProperty(ref _shippedDate, value);
        }
        public DateTimeOffset? DeliveredDate
        {
            get => _deliveredDate;
            set => SetProperty(ref _deliveredDate, value);
        }
        public string TrackingNumber { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipPhone { get; set; }


        public long CustomerId { get; set; }
        public int? PaymentTypeId { get; set; }
        public long ShipCountryId { get; set; }
        public int? ShipperId { get; set; }
        public int StatusId
        {
            get => _status;
            set { if (SetProperty(ref _status, value)) UpdateStatusDependencies(); }
        }

        public CustomerDto Customer { get; set; }
        public PaymentTypeDto PaymentType { get; set; }
        public CountryDto ShipCountry { get; set; }
        public ShipperDto Shipper { get; set; }
        public OrderStatusDto Status { get; set; }
        public IList<OrderItemDto> OrderItems { get; set; }


        public bool IsNew => Id <= 0;
        public bool CanEditPayment => StatusId > 0;
        public bool CanEditShipping => StatusId > 1;
        public bool CanEditDelivery => StatusId > 2;

        public string StatusDesc { get; set; }
        public string PaymentTypeDesc { get; set; }
        public string ShipViaDesc { get; set; }
        public string ShipCountryName { get; set; }


        private void UpdateStatusDependencies()
        {
            switch (StatusId)
            {
                case 0:
                case 1:
                    ShippedDate = null;
                    DeliveredDate = null;
                    break;
                case 2:
                    ShippedDate = ShippedDate ?? OrderDate;
                    DeliveredDate = null;
                    break;
                case 3:
                    ShippedDate = ShippedDate ?? OrderDate;
                    DeliveredDate = DeliveredDate ?? ShippedDate ?? OrderDate;
                    break;
            }

            OnPropertyChanged(nameof(StatusDesc));
            OnPropertyChanged(nameof(CanEditPayment));
            OnPropertyChanged(nameof(CanEditShipping));
            OnPropertyChanged(nameof(CanEditDelivery));
        }

        public override void Merge(ObservableDto source)
        {
            if (source is OrderDto model)
            {
                Merge(model);
            }
        }

        public void Merge(OrderDto source)
        {
            if (source != null)
            {
                Id = source.Id;
                OrderDate = source.OrderDate;
                ShippedDate = source.ShippedDate;
                DeliveredDate = source.DeliveredDate;
                PaymentTypeId = source.PaymentTypeId;
                TrackingNumber = source.TrackingNumber;
                ShipAddress = source.ShipAddress;
                ShipCity = source.ShipCity;
                ShipRegion = source.ShipRegion;
                ShipPostalCode = source.ShipPostalCode;
                ShipPhone = source.ShipPhone;

                CustomerId = source.CustomerId;
                PaymentTypeId = source?.PaymentTypeId;
                ShipCountryId = source.ShipCountryId;
                ShipperId = source.ShipperId;
                StatusId = source.StatusId;

                Customer = source.Customer;
                PaymentType = source?.PaymentType;
                ShipCountry = source?.ShipCountry;
                Shipper = source?.Shipper;
                Status = source?.Status;
            }
        }



        public override string ToString()
        {
            return Id.ToString();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as OrderDto);
        }
        public bool Equals(OrderDto other)
        {
            return !(other is null) &&
                   Id == other.Id;
        }
        public override int GetHashCode()
        {
            return 1651275338 + Id.GetHashCode();
        }
    }
}
