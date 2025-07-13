using Inventory.Infrastructure;
using Inventory.Infrastructure.Settings;
using Inventory.Uwp.Configuration;
using Inventory.Uwp.Services;
using Inventory.Uwp.Services.VirtualCollections;
using Inventory.Uwp.ViewModels;
using Inventory.Uwp.ViewModels.Customers;
using Inventory.Uwp.ViewModels.Dashboard;
using Inventory.Uwp.ViewModels.Logs;
using Inventory.Uwp.ViewModels.OrderItems;
using Inventory.Uwp.ViewModels.Orders;
using Inventory.Uwp.ViewModels.Products;
using Inventory.Uwp.ViewModels.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Uwp
{
    public static class PresentationRegister
    {
        public static void Register(IServiceCollection serviceCollection)
        {
            serviceCollection

           // Services
           .AddScoped<NavigationService>()
           .AddScoped<WindowManagerService>()
           .AddSingleton<AppSettings>()
           .AddSingleton<IAppSettings>(x => x.GetRequiredService<AppSettings>())
           .AddSingleton<ISettingsService, LocalSettingsService>()

           .AddSingleton<FilePickerService>()

           // ViewModels
           .AddTransient<ShellViewModel>()
           .AddTransient<DashboardViewModel>()
           .AddTransient<SettingsViewModel>()
           .AddTransient<ValidateConnectionViewModel>()
           .AddTransient<CreateDatabaseViewModel>()
           .AddTransient<CustomerDetailsViewModel>()
           .AddTransient<CustomerListViewModel>()
           .AddTransient<CustomersViewModel>()
           .AddTransient<ProductListViewModel>()
           .AddTransient<ProductDetailsViewModel>()
           .AddTransient<ProductsViewModel>()
           .AddTransient<OrderDetailsViewModel>()
           .AddTransient<OrderDetailsWithItemsViewModel>()
           .AddTransient<OrderListViewModel>()
           .AddTransient<OrdersViewModel>()
           .AddTransient<OrderItemDetailsViewModel>()
           .AddTransient<OrderItemListViewModel>()
           .AddTransient<OrderItemsViewModel>()
           .AddTransient<LogsViewModel>()
           .AddTransient<LogListViewModel>()
           .AddTransient<LogDetailsViewModel>()
           .AddTransient<CustomerCollection>()
           .AddTransient<LogCollection>()
           .AddTransient<OrderCollection>()
           .AddTransient<ProductCollection>();
        }
    }
}
