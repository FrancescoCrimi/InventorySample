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
using Inventory.Data.DataContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Inventory.Services.Infrastructure.LogService;
using Microsoft.Extensions.Logging.Configuration;

namespace Inventory
{
    sealed partial class App : Application
    {
        //private readonly ILogger logger;

        public App()
        {
            InitializeComponent();

            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            //ApplicationView.PreferredLaunchViewSize = new Size(1280, 840);

            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;

            Ioc.Default.ConfigureServices(ConfigureServices());
            //logger = Ioc.Default.GetRequiredService<ILogger<App>>();
            //var factory = Ioc.Default.GetService<ILoggerFactory>();
            //logger = factory.CreateLogger(typeof(App).Name);
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

        private  void OnSuspending(object sender, SuspendingEventArgs e)
        {
            //var logService = Ioc.Default.GetService<ILogService>();
            //await logService.WriteAsync(Data.LogType.Information, "App", "Suspending", "Application End", $"Application ended by '{AppSettings.Current.UserName}'.");
            //logger.LogInformation($"Application ended by '{AppSettings.Current.UserName}'.");
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            //var logService = Ioc.Default.GetService<ILogService>();
            //logService.WriteAsync(Data.LogType.Error, "App", "UnhandledException", e.Message, e.Exception.ToString());
            //logger.LogError("UnhandledException: " + e.Message);
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services
                .AddLogging(builder => {
                    //builder.ClearProviders();
                    //builder.AddConfiguration();
                    builder.Services.TryAddEnumerable(
                        ServiceDescriptor.Singleton<ILoggerProvider, DatabaseLoggerProvider>());
                    //builder.Services.Configure()
                })
                .AddDbContext<AppLogDbContext>(option =>
                {
                    option.UseSqlite(AppSettings.Current.AppLogConnectionString);
                })
            .AddSingleton<ISettingsService, SettingsService>()
            .AddSingleton<IDataServiceFactory, DataServiceFactory>()
            .AddSingleton<ILookupTables, LookupTables>()
            .AddSingleton<ICustomerService, CustomerService>()
            .AddSingleton<IOrderService, OrderService>()
            .AddSingleton<IOrderItemService, OrderItemService>()
            .AddSingleton<IProductService, ProductService>()

            .AddSingleton<IMessageService, MessageService>()
            //.AddSingleton<ILogService, LogService>()
            .AddSingleton<IDialogService, DialogService>()
            .AddSingleton<IFilePickerService, FilePickerService>()
            //.AddSingleton<ILoginService, LoginService>()

            .AddScoped<IContextService, ContextService>()
            .AddScoped<INavigationService, NavigationService>()
            //.AddScoped<ICommonServices, CommonServices>()

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
