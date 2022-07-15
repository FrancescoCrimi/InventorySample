#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using CiccioSoft.Inventory.Application;
using CiccioSoft.Inventory.Infrastructure;
using CiccioSoft.Inventory.Uwp.Activation;
using CiccioSoft.Inventory.Uwp.Services;
using CiccioSoft.Inventory.Uwp.Services.Infrastructure;
using CiccioSoft.Inventory.Uwp.ViewModels;
using CiccioSoft.Inventory.Uwp.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;

namespace CiccioSoft.Inventory.Uwp
{
    sealed partial class App : Windows.UI.Xaml.Application
    {
        private ActivationService ActivationService => new ActivationService();

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;
            Ioc.Default.ConfigureServices(ConfigureServices());
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var logger = Ioc.Default.GetService<ILogger<App>>();
            logger.LogInformation($"Application ended by '{AppSettings.Current.UserName}'.");
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            var logger = Ioc.Default.GetService<ILogger<App>>();
            logger.LogError("UnhandledException: " + e.Message);
        }



        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()

                .AddSingleton<DefaultActivationHandler>()
                .AddSingleton<IAppSettings, AppSettings>()
                .AddInventoryData()

                .AddSingleton<ProductServiceUwp>()
                .AddSingleton<CustomerServiceUwp>()
                .AddSingleton<OrderServiceUwp>()
                .AddSingleton<OrderItemServiceUwp>()

                //.AddSingleton<IMessageService, MessageService>()
                .AddSingleton<FilePickerService, FilePickerService>()

                .AddScoped<NavigationService, NavigationService>()
                .AddSingleton<PageService>()
                .AddSingleton<WindowService, WindowService>()

                .AddTransient<ShellView>()
                .AddTransient<ShellViewModel>()

                .AddTransient<DashboardViewModel>()
                .AddTransient<SettingsViewModel>()
                .AddTransient<ValidateConnectionViewModel>()
                .AddTransient<CreateDatabaseViewModel>()

                .AddTransient<CustomerListViewModel>()
                .AddTransient<CustomerDetailsViewModel>()
                .AddTransient<CustomersViewModel>()

                .AddTransient<ProductListViewModel>()
                .AddTransient<ProductDetailsViewModel>()
                .AddTransient<ProductsViewModel>()

                .AddTransient<OrderDetailsViewModel>()
                .AddTransient<OrderDetailsWithItemsViewModel>()
                .AddTransient<OrderListViewModel>()
                .AddTransient<OrdersViewModel>()

                .AddTransient<OrderItemListViewModel>()
                .AddTransient<OrderItemDetailsViewModel>()
                .AddTransient<OrderItemsViewModel>()

                .AddTransient<AppLogsViewModel>()
                .AddTransient<AppLogListViewModel>()
                .AddTransient<AppLogDetailsViewModel>()

                .BuildServiceProvider();
        }
    }
}
