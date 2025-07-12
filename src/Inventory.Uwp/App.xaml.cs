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
using Inventory.Infrastructure.Settings;
using Inventory.Interface;
using Inventory.Interface.Services;
using Inventory.Persistence;
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
using Inventory.Uwp.Views.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Inventory.Uwp
{
    public sealed partial class App : Windows.UI.Xaml.Application
    {
        private ILogger _logger;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;
            Ioc.Default.ConfigureServices(ConfigureServices());
            _logger = Ioc.Default.GetService<ILogger<App>>();
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivateAsync(args);
        }

        public async Task ActivateAsync(IActivatedEventArgs activationArgs)
        {
            await InitializeAsync();
            if (Window.Current.Content == null)
            {
                Window.Current.Content = new Views.ShellPage();
            }

            object arguments = null;
            if (activationArgs is LaunchActivatedEventArgs launchArgs)
            {
                arguments = launchArgs.Arguments;
            }
            Ioc.Default.GetService<NavigationService>().Navigate(typeof(DashboardView), arguments);

            Window.Current.Activate();
            await StartupAsync();
        }

        private async Task InitializeAsync()
        {
            ThemeSelectorService.Initialize();
            var appSettings = Ioc.Default.GetService<AppSettings>();
            await appSettings.EnsureLogDatabaseAsync();
            await appSettings.EnsureLocalDatabaseAsync();
            await Ioc.Default.GetService<LookupTablesServiceFacade>().InitializeAsync();
            _logger.LogInformation(LogEvents.Startup, "Application Started");
        }

        private async Task StartupAsync()
        {
            await ThemeSelectorService.SetRequestedThemeAsync();
        }


        private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            _logger.LogInformation(LogEvents.Suspending, $"Application ended.");
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            _logger.LogError(LogEvents.UnhandledException, e.Exception, "Unhandled Exception");
        }

        private IServiceProvider ConfigureServices()
            => new ServiceCollection()

            // Services
            .AddScoped<NavigationService>()
            .AddScoped<WindowManagerService>()
            .AddSingleton<AppSettings>()
            .AddSingleton<IAppSettings>(x => x.GetRequiredService<AppSettings>())
            .AddSingleton<ISettingsService, LocalSettingsService>()

            // Core Services
            .AddInventoryInterface()

            //.AddSingleton<LookupTablesService>()
            //.AddSingleton<ProductService>()
            //.AddSingleton<CustomerService>()
            //.AddSingleton<OrderService>()
            //.AddSingleton<OrderItemService>()

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
