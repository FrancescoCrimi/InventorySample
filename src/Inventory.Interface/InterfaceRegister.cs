using Inventory.Interface.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Interface
{
    public static class InterfaceRegister
    {
        public static void Register(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<LookupTablesServiceFacade>()
                .AddSingleton<ProductServiceFacade>()
                .AddSingleton<CustomerServiceFacade>()
                .AddSingleton<OrderServiceFacade>()
                .AddSingleton<OrderItemServiceFacade>();
        }
    }
}
