// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Logging;
using Inventory.Persistence;
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
            logger.LogInformation(LogEvents.Suspending, $"Application ended.");
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            var logger = Ioc.Default.GetService<ILogger<App>>();
            logger.LogError(LogEvents.UnhandledException, e.Exception, "Unhandled Exception");
        }

        private IServiceProvider ConfigureServices() => new ServiceCollection()

            // Default Activation Handler
            .AddTransient<ActivationHandler<IActivatedEventArgs>, DefaultActivationHandler>()

            // Other Activation Handlers

            // Services
            .AddSingleton<ActivationService>()
            .AddScoped<NavigationService>()
            .AddSingleton<WindowManagerService>()
            .AddSingleton<AppSettings>()
            .AddSingleton<IAppSettings>(x => x.GetRequiredService<AppSettings>())

            // Core Services
            .AddInventoryInfrastructure()
            .AddInventoryPersistence()
            .AddSingleton<LookupTablesService>()
            .AddSingleton<ProductService>()
            .AddSingleton<CustomerService>()
            .AddSingleton<OrderService>()
            .AddSingleton<OrderItemService>()
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
