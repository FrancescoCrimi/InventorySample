using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Application
{
    public static class ApplicationRegister
    {
        public static void Register(IServiceCollection serviceCollection)
        {
             serviceCollection
                .AddSingleton<LookupTablesService>()
                .AddSingleton<ProductService>()
                .AddSingleton<CustomerService>()
                .AddSingleton<OrderService>()
                .AddSingleton<OrderItemService>();
        }
    }
}
