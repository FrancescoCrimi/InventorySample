using Inventory.Domain.Model;
using Inventory.Uwp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Uwp.Dto
{
    public static class DtoAssembler
    {
        public static async Task<CustomerDto> CreateCustomerModelAsync(Customer source, bool includeAllFields, Windows.UI.Core.CoreDispatcher dispatcher = null)
        {
            var model = new CustomerDto()
            {
                CustomerID = source.Id,
                Title = source.Title,
                FirstName = source.FirstName,
                MiddleName = source.MiddleName,
                LastName = source.LastName,
                Suffix = source.Suffix,
                Gender = source.Gender,
                EmailAddress = source.EmailAddress,
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                City = source.City,
                Region = source.Region,
                CountryCode = source.CountryCode,
                PostalCode = source.PostalCode,
                Phone = source.Phone,
                CreatedOn = source.CreatedOn,
                LastModifiedOn = source.LastModifiedOn,
                Thumbnail = source.Thumbnail
                //ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail)
            };

            if (dispatcher != null)
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    model.ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail);
                });
            }
            else
                model.ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail);

            if (includeAllFields)
            {
                model.BirthDate = source.BirthDate;
                model.Education = source.Education;
                model.Occupation = source.Occupation;
                model.YearlyIncome = source.YearlyIncome;
                model.MaritalStatus = source.MaritalStatus;
                model.TotalChildren = source.TotalChildren;
                model.ChildrenAtHome = source.ChildrenAtHome;
                model.IsHouseOwner = source.IsHouseOwner;
                model.NumberCarsOwned = source.NumberCarsOwned;
                model.Picture = source.Picture;
                model.PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture);
            }
            return model;
        }

        public static void UpdateCustomerFromModel(Customer target, CustomerDto source)
        {
            target.Title = source.Title;
            target.FirstName = source.FirstName;
            target.MiddleName = source.MiddleName;
            target.LastName = source.LastName;
            target.Suffix = source.Suffix;
            target.Gender = source.Gender;
            target.EmailAddress = source.EmailAddress;
            target.AddressLine1 = source.AddressLine1;
            target.AddressLine2 = source.AddressLine2;
            target.City = source.City;
            target.Region = source.Region;
            target.CountryCode = source.CountryCode;
            target.PostalCode = source.PostalCode;
            target.Phone = source.Phone;
            target.BirthDate = source.BirthDate;
            target.Education = source.Education;
            target.Occupation = source.Occupation;
            target.YearlyIncome = source.YearlyIncome;
            target.MaritalStatus = source.MaritalStatus;
            target.TotalChildren = source.TotalChildren;
            target.ChildrenAtHome = source.ChildrenAtHome;
            target.IsHouseOwner = source.IsHouseOwner;
            target.NumberCarsOwned = source.NumberCarsOwned;
            target.CreatedOn = source.CreatedOn;
            target.LastModifiedOn = source.LastModifiedOn;
            target.Picture = source.Picture;
            target.Thumbnail = source.Thumbnail;
        }


        public static async Task<ProductDto> CreateProductModelAsync(Product source, bool includeAllFields, Windows.UI.Core.CoreDispatcher dispatcher)
        {
            var model = new ProductDto()
            {
                ProductID = source.Id,
                CategoryID = source.CategoryID,
                Name = source.Name,
                Description = source.Description,
                Size = source.Size,
                Color = source.Color,
                ListPrice = source.ListPrice,
                DealerPrice = source.DealerPrice,
                TaxType = source.TaxType,
                Discount = source.Discount,
                DiscountStartDate = source.DiscountStartDate,
                DiscountEndDate = source.DiscountEndDate,
                StockUnits = source.StockUnits,
                SafetyStockLevel = source.SafetyStockLevel,
                CreatedOn = source.CreatedOn,
                LastModifiedOn = source.LastModifiedOn,
                Thumbnail = source.Thumbnail,
                //ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail)
            };
            if (dispatcher != null)
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    model.ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail);
                });
            }
            else
                model.ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail);

            if (includeAllFields)
            {
                model.Picture = source.Picture;
                if (dispatcher != null)
                {
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        model.PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture);
                    });
                }
                else
                    model.PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture);
            }
            return model;
        }

        public static void UpdateProductFromModel(Product target, ProductDto source)
        {
            target.CategoryID = source.CategoryID;
            target.Name = source.Name;
            target.Description = source.Description;
            target.Size = source.Size;
            target.Color = source.Color;
            target.ListPrice = source.ListPrice;
            target.DealerPrice = source.DealerPrice;
            target.TaxType = source.TaxType;
            target.Discount = source.Discount;
            target.DiscountStartDate = source.DiscountStartDate;
            target.DiscountEndDate = source.DiscountEndDate;
            target.StockUnits = source.StockUnits;
            target.SafetyStockLevel = source.SafetyStockLevel;
            target.CreatedOn = source.CreatedOn;
            target.LastModifiedOn = source.LastModifiedOn;
            target.Picture = source.Picture;
            target.Thumbnail = source.Thumbnail;
        }


        public static async Task<OrderDto> CreateOrderModelAsync(Order source, bool includeAllFields, Windows.UI.Core.CoreDispatcher dispatcher)
        {
            var model = new OrderDto()
            {
                OrderID = source.Id,
                CustomerID = source.CustomerID,
                OrderDate = source.OrderDate,
                ShippedDate = source.ShippedDate,
                DeliveredDate = source.DeliveredDate,
                Status = source.Status,
                PaymentType = source.PaymentType,
                TrackingNumber = source.TrackingNumber,
                ShipVia = source.ShipVia,
                ShipAddress = source.ShipAddress,
                ShipCity = source.ShipCity,
                ShipRegion = source.ShipRegion,
                ShipCountryCode = source.ShipCountryCode,
                ShipPostalCode = source.ShipPostalCode,
                ShipPhone = source.ShipPhone,
            };
            if (source.Customer != null)
            {
                model.Customer = await CreateCustomerModelAsync(source.Customer, includeAllFields, null);
            }
            return model;
        }

        public static void UpdateOrderFromModel(Order target, OrderDto source)
        {
            target.CustomerID = source.CustomerID;
            target.OrderDate = source.OrderDate;
            target.ShippedDate = source.ShippedDate;
            target.DeliveredDate = source.DeliveredDate;
            target.Status = source.Status;
            target.PaymentType = source.PaymentType;
            target.TrackingNumber = source.TrackingNumber;
            target.ShipVia = source.ShipVia;
            target.ShipAddress = source.ShipAddress;
            target.ShipCity = source.ShipCity;
            target.ShipRegion = source.ShipRegion;
            target.ShipCountryCode = source.ShipCountryCode;
            target.ShipPostalCode = source.ShipPostalCode;
            target.ShipPhone = source.ShipPhone;
        }


        public static async Task<OrderItemDto> CreateOrderItemModelAsync(OrderItem source, bool includeAllFields)
        {
            var model = new OrderItemDto()
            {
                OrderID = source.OrderID,
                OrderLine = source.OrderLine,
                ProductID = source.ProductID,
                Quantity = source.Quantity,
                UnitPrice = source.UnitPrice,
                Discount = source.Discount,
                TaxType = source.TaxType,
                Product = await CreateProductModelAsync(source.Product, includeAllFields, null)
            };
            return model;
        }

        public static void UpdateOrderItemFromModel(OrderItem target, OrderItemDto source)
        {
            target.OrderID = source.OrderID;
            target.OrderLine = source.OrderLine;
            target.ProductID = source.ProductID;
            target.Quantity = source.Quantity;
            target.UnitPrice = source.UnitPrice;
            target.Discount = source.Discount;
            target.TaxType = source.TaxType;
        }
    }
}
