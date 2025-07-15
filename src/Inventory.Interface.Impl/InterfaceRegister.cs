using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Interface.Impl
{
    public static class InterfaceRegister
    {
        public static void Register(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<ILookupTablesServiceFacade, LookupTablesServiceFacade>()
                .AddSingleton<IProductServiceFacade, ProductServiceFacade>()
                .AddSingleton<ICustomerServiceFacade, CustomerServiceFacade>()
                .AddSingleton<IOrderServiceFacade, OrderServiceFacade>()
                .AddSingleton<IOrderItemServiceFacade, OrderItemServiceFacade>();
        }
    }
}
