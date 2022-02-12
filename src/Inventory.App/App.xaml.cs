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

using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Inventory.Services;
using Inventory.Views.SplashScreen;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection;
using Inventory.ViewModels;

namespace Inventory
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.PreferredLaunchViewSize = new Size(1280, 840);

            Ioc.Default.ConfigureServices(ConfigureServices());
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                ExtendedSplash extendedSplash = new ExtendedSplash(e);
                Window.Current.Content = extendedSplash;
            }

            if (e.PrelaunchActivated == false)
            {
                Window.Current.Activate();
            }
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var logService = Ioc.Default.GetService<ILogService>();
            await logService.WriteAsync(Data.LogType.Information, "App", "Suspending", "Application End", $"Application ended by '{AppSettings.Current.UserName}'.");
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            var logService = Ioc.Default.GetService<ILogService>();
            logService.WriteAsync(Data.LogType.Error, "App", "UnhandledException", e.Message, e.Exception.ToString());
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services
                .AddLogging()

            .AddSingleton<ISettingsService, SettingsService>()
            .AddSingleton<IDataServiceFactory, DataServiceFactory>()
            .AddSingleton<ILookupTables, LookupTables>()
            .AddSingleton<ICustomerService, CustomerService>()
            .AddSingleton<IOrderService, OrderService>()
            .AddSingleton<IOrderItemService, OrderItemService>()
            .AddSingleton<IProductService, ProductService>()

            .AddSingleton<IMessageService, MessageService>()
            .AddSingleton<ILogService, LogService>()
            .AddSingleton<IDialogService, DialogService>()
            .AddSingleton<IFilePickerService, FilePickerService>()
            //.AddSingleton<ILoginService, LoginService>()

            .AddScoped<IContextService, ContextService>()
            .AddScoped<INavigationService, NavigationService>()
            .AddScoped<ICommonServices, CommonServices>()

            .AddTransient<LoginViewModel>()

            .AddTransient<ShellViewModel>()
            .AddTransient<MainShellViewModel>()

            .AddTransient<DashboardViewModel>()

            .AddTransient<CustomersViewModel>()
            .AddTransient<CustomerDetailsViewModel>()

            .AddTransient<OrdersViewModel>()
            .AddTransient<OrderDetailsViewModel>()
            .AddTransient<OrderDetailsWithItemsViewModel>()

            .AddTransient<OrderItemsViewModel>()
            .AddTransient<OrderItemDetailsViewModel>()

            .AddTransient<ProductsViewModel>()
            .AddTransient<ProductDetailsViewModel>()

            .AddTransient<AppLogsViewModel>()

            .AddTransient<SettingsViewModel>()
            .AddTransient<ValidateConnectionViewModel>()
            .AddTransient<CreateDatabaseViewModel>()



            .AddTransient<CustomerListViewModel>()
            .AddTransient<CustomerDetailsViewModel>()
            .AddTransient<OrderListViewModel>()

            .AddTransient<ProductListViewModel>()
            .AddTransient<ProductDetailsViewModel>()

            .AddTransient<OrderListViewModel>()
            .AddTransient<OrderDetailsViewModel>()
            .AddTransient<OrderItemListViewModel>()

            .AddTransient<OrderItemListViewModel>()
            .AddTransient<OrderItemDetailsViewModel>()

            .AddTransient<OrderDetailsViewModel>()
            .AddTransient<OrderItemListViewModel>()

            .AddTransient<AppLogListViewModel>()
            .AddTransient<AppLogDetailsViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
