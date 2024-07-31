using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Domain.Model;
using Inventory.Interface.Services;

namespace Inventory.Interface.Dto
{
    public static partial class DtoAssembler
    {
        static readonly LookupTablesServiceFacade _lookupTable;

        static DtoAssembler()
        {
            _lookupTable = Ioc.Default.GetService<LookupTablesServiceFacade>();
        }




        public static OrderItemDto CreateOrderItemModelAsync(OrderItem source, bool includeAllFields)
        {
            var model = new OrderItemDto()
            {
                OrderId = source.OrderId,
                OrderLine = source.OrderLine,
                ProductId = source.ProductId,
                //TaxTypeId = source.TaxTypeId,

                Quantity = source.Quantity,
                UnitPrice = source.UnitPrice,
                Discount = source.Discount,

                TaxType =  source.TaxType,
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
