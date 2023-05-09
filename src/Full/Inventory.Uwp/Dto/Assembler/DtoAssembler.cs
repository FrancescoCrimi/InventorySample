using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Domain.Model;
using Inventory.Uwp.Services;

namespace Inventory.Uwp.Dto
{
    public static partial class DtoAssembler
    {
        static readonly LookupTablesService _lookupTable;

        static DtoAssembler()
        {
            _lookupTable = Ioc.Default.GetService<LookupTablesService>();
        }




        public static OrderItemDto CreateOrderItemModelAsync(OrderItem source, bool includeAllFields)
        {
            var model = new OrderItemDto()
            {
                OrderId = source.OrderId,
                OrderLine = source.OrderLine,
                ProductId = source.ProductId,
                Quantity = source.Quantity,
                UnitPrice = source.UnitPrice,
                Discount = source.Discount,
                TaxType = new TaxTypeDto { Id = source.TaxType.Id, Name = source.TaxType.Name, Rate = source.TaxType.Rate },
                Product = DtoFromProduct(source.Product, includeAllFields)
            };
            return model;
        }

        public static void UpdateOrderItemFromModel(OrderItem target, OrderItemDto source)
        {
            target.OrderId = source.OrderId;
            target.OrderLine = source.OrderLine;
            target.ProductId = source.ProductId;
            target.Quantity = source.Quantity;
            target.UnitPrice = source.UnitPrice;
            target.Discount = source.Discount;
            //target.TaxType = source.TaxType;
        }
    }
}
