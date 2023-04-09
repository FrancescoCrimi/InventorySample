using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.Uwp.Activation;
using Inventory.Uwp.Services;
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
using System;
using Windows.ApplicationModel.Activation;

namespace Inventory.Uwp
{
    public sealed partial class App : Windows.UI.Xaml.Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;
            Ioc.Default.ConfigureServices(ConfigureServices());
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await Ioc.Default.GetService<ActivationService>().ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await Ioc.Default.GetService<ActivationService>().ActivateAsync(args);
        }

        private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var logger = Ioc.Default.GetService<ILogger<App>>();
            logger.LogInformation($"Application ended.");
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
            var logger = Ioc.Default.GetService<ILogger<App>>();
            logger.LogError("UnhandledException: " + e.Message);
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
                .AddSingleton<WindowManagerService, WindowManagerService>()

                .AddSingleton<IAppSettings, AppSettings>()

                // Core Services
                .AddInventoryApplication()

                .AddTransient<LogServiceFacade>()
                .AddSingleton<LookupTableServiceFacade>()
                .AddSingleton<ProductServiceFacade>()
                .AddSingleton<CustomerServiceFacade>()
                .AddSingleton<OrderServiceFacade>()
                .AddSingleton<OrderItemServiceFacade>()

                ////.AddSingleton<IMessageService, MessageService>()
                .AddSingleton<FilePickerService, FilePickerService>()

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

                .BuildServiceProvider();
        }
    }
}
