using Inventory.Domain.Model;
using System.Linq;

namespace Inventory.Interface.Dto
{
    public static partial class DtoAssembler
    {
        public static OrderDto DtoFromOrder(Order source)
        {
            var model = new OrderDto()
            {
                Id = source.Id,
                OrderDate = source.OrderDate,
                ShippedDate = source.ShippedDate,
                DeliveredDate = source.DeliveredDate,
                TrackingNumber = source.TrackingNumber,
                ShipAddress = source.ShipAddress,
                ShipCity = source.ShipCity,
                ShipRegion = source.ShipRegion,
                ShipPostalCode = source.ShipPostalCode,
                ShipPhone = source.ShipPhone,

                CustomerId = source.CustomerId,
                PaymentTypeId = source.PaymentTypeId,
                ShipCountryId = source.ShipCountryId,
                ShipperId = source.ShipperId,
                StatusId = source.StatusId,

                PaymentType = _lookupTable.PaymentTypes.FirstOrDefault(p => p.Id == source.PaymentTypeId),
                ShipCountry = _lookupTable.Countries.FirstOrDefault(c => c.Id == source.ShipCountryId),
                Shipper = _lookupTable.Shippers.FirstOrDefault(s => s.Id == source.ShipperId),
                Status = _lookupTable.OrderStatus.FirstOrDefault(s => s.Id == source.StatusId),
            };

            if (source.Customer != null)
            {
                model.Customer = DtoFromCustomer(source.Customer);
            }

            model.PaymentTypeDesc = model.PaymentType.Name;
            model.ShipCountryName = model.ShipCountry.Name;
            model.ShipViaDesc = model.Shipper?.Name;
            model.StatusDesc = model.Status.Name;

            return model;
        }

        public static void UpdateOrderFromDto(Order target, OrderDto source)
        {
            target.OrderDate = source.OrderDate;
            target.ShippedDate = source.ShippedDate;
            target.DeliveredDate = source.DeliveredDate;
            target.TrackingNumber = source.TrackingNumber;
            target.ShipAddress = source.ShipAddress;
            target.ShipCity = source.ShipCity;
            target.ShipRegion = source.ShipRegion;
            target.ShipPostalCode = source.ShipPostalCode;
            target.ShipPhone = source.ShipPhone;

            target.CustomerId = source.CustomerId;
            target.PaymentTypeId = source.PaymentTypeId;
            target.ShipCountryId = source.ShipCountryId;
            target.ShipperId = source.ShipperId;
            target.StatusId = source.StatusId;
        }
    }
}
