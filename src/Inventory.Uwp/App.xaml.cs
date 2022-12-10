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
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;

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
            Task.Run(async () =>
            {
                await EnsureLogDbAsync();
                await EnsureDatabaseAsync();
            });
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            await Ioc.Default.GetService<ActivationService>().ActivateAsync(args);
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await Ioc.Default.GetService<ActivationService>().ActivateAsync(args);
        }

        private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var logger = Ioc.Default.GetService<ILogger<App>>();
            logger.LogInformation($"Application ended by '{AppSettings.Current.UserName}'.");
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
                .AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>()

                // Other Activation Handlers

                // Services
                .AddSingleton<ActivationService>()
                .AddScoped<NavigationService>()
                .AddSingleton<PageService>()
                .AddSingleton<WindowService, WindowService>()

                .AddSingleton<IAppSettings, AppSettings>()
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

        private async Task EnsureLogDbAsync()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var appLogFolder = await localFolder.CreateFolderAsync(AppSettings.AppLogPath, CreationCollisionOption.OpenIfExists);
            if (await appLogFolder.TryGetItemAsync(AppSettings.AppLogName) == null)
            {
                var sourceLogFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/AppLog/AppLog.db"));
                var targetLogFile = await appLogFolder.CreateFileAsync(AppSettings.AppLogName, CreationCollisionOption.ReplaceExisting);
                await sourceLogFile.CopyAndReplaceAsync(targetLogFile);
                //CreateSqliteLogDb();
            }
        }

        private async Task EnsureDatabaseAsync()
        {
            await EnsureSQLiteDatabaseAsync();
        }

        private async Task EnsureSQLiteDatabaseAsync()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var databaseFolder = await localFolder.CreateFolderAsync(AppSettings.DatabasePath, CreationCollisionOption.OpenIfExists);
            if (await databaseFolder.TryGetItemAsync(AppSettings.DatabaseName) == null)
            {
                var sourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Database/VanArsdel.1.01.db"));
                var targetFile = await databaseFolder.CreateFileAsync(AppSettings.DatabaseName, CreationCollisionOption.ReplaceExisting);
                await sourceFile.CopyAndReplaceAsync(targetFile);
            }
        }
    }
}
