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
    public class OrderDto : ObservableDto<OrderDto>
    {
        private DateTimeOffset _orderDate;
        private DateTimeOffset? _shippedDate;
        private DateTimeOffset? _deliveredDate;
        private string _trackingNumber;
        private string _shipAddress;
        private string _shipCity;
        private string _shipRegion;
        private string _shipPostalCode;
        private string _shipPhone;
        private long _customerId;
        private long? _paymentTypeId;
        private long _shipCountryId;
        private long? _shipperId;
        private long _statusId;
        public CustomerDto _customer;
        public PaymentTypeDto _paymentType;
        public CountryDto _shipCountry;
        public ShipperDto _shipper;
        private OrderStatusDto _status;

        #region property

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
        public string TrackingNumber
        {
            get => _trackingNumber;
            set => SetProperty(ref _trackingNumber, value);
        }
        public string ShipAddress
        {
            get => _shipAddress;
            set => SetProperty(ref _shipAddress, value);
        }
        public string ShipCity
        {
            get => _shipCity;
            set => SetProperty(ref _shipCity, value);
        }
        public string ShipRegion
        {
            get => _shipRegion;
            set => SetProperty(ref _shipRegion, value);
        }
        public string ShipPostalCode
        {
            get => _shipPostalCode;
            set => SetProperty(ref _shipPostalCode, value);
        }
        public string ShipPhone
        {
            get => _shipPhone;
            set => SetProperty(ref _shipPhone, value);
        }

        #endregion


        #region relation

        public long CustomerId
        {
            get => _customerId;
            set => SetProperty(ref _customerId, value);
        }
        public long? PaymentTypeId
        {
            get => _paymentTypeId;
            set => SetProperty(ref _paymentTypeId, value);
        }
        public long ShipCountryId
        {
            get => _shipCountryId;
            set => SetProperty(ref _shipCountryId, value);
        }
        public long? ShipperId
        {
            get => _shipperId;
            set => SetProperty(ref _shipperId, value);
        }
        public long StatusId
        {
            get => _statusId;
            set
            {
                if (SetProperty(ref _statusId, value))
                    UpdateStatusDependencies();
            }
        }

        public CustomerDto Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
        }
        public PaymentTypeDto PaymentType
        {
            get => _paymentType;
            set => SetProperty(ref _paymentType, value);
        }
        public CountryDto ShipCountry
        {
            get => _shipCountry;
            set => SetProperty(ref _shipCountry, value);
        }
        public ShipperDto Shipper
        {
            get => _shipper;
            set => SetProperty(ref _shipper, value);
        }
        public OrderStatusDto Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value))
                    UpdateStatusDependencies();
            }
        }
        public IList<OrderItemDto> OrderItems { get; set; }

        #endregion


        public bool CanEditPayment => StatusId > 0L;
        public bool CanEditShipping => StatusId > 1L;
        public bool CanEditDelivery => StatusId > 2L;

        public string StatusDesc { get; set; }
        public string PaymentTypeDesc { get; set; }
        public string ShipViaDesc { get; set; }
        public string ShipCountryName { get; set; }


        #region method

        public static OrderDto CreateEmpty() => new OrderDto { Id = -1, CustomerId = -1, IsEmpty = true };

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

        public override void Merge(OrderDto source)
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

        #endregion
    }
}
