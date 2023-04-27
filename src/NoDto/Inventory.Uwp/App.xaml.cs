using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Persistence;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Activation;
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
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;

namespace Inventory.Uwp
{
    sealed partial class App : Windows.UI.Xaml.Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;
            Ioc.Default.ConfigureServices(ConfigureServices());
        }

        protected async override void OnActivated(IActivatedEventArgs e)
        {
            await Ioc.Default.GetService<ActivationService>().ActivateAsync(e);
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!e.PrelaunchActivated)
            {
                await Ioc.Default.GetService<ActivationService>().ActivateAsync(e);
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var logger = Ioc.Default.GetService<ILogger<App>>();
            //await logService.WriteAsync(Data.LogType.Information, "App", "Suspending", "Application End", $"Application ended by '{AppSettings.Current.UserName}'.");
            logger.LogInformation(LogEvents.Suspending, $"Application ended.");
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            var logger = Ioc.Default.GetService<ILogger<App>>();
            //logService.WriteAsync(Data.LogType.Error, "App", "UnhandledException", e.Message, e.Exception.ToString());
            logger.LogError(LogEvents.UnhandledException, e.Exception, "Unhandled Exception");
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()

                // Default Activation Handler
                .AddTransient<ActivationHandler<IActivatedEventArgs>, DefaultActivationHandler>()

                // Other Activation Handlers

                // Services
                .AddSingleton<ActivationService>()
                .AddScoped<NavigationService>()
                .AddSingleton<WindowManagerService>()

                .AddSingleton<IAppSettings, AppSettings>()

                // Core Services
                //.AddInventoryApplication()
                .AddInventoryInfrastructure()
                .AddInventoryPersistence()

                //.AddTransient<LogServiceFacade>()
                .AddSingleton<LookupTablesService>()

                ////.AddSingleton<IMessageService, MessageService>()
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
                .AddTransient<ProductCollection>()

                .BuildServiceProvider();
        }
    }
}
