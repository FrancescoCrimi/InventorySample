using CiccioSoft.Inventory.Application;
using CiccioSoft.Inventory.Infrastructure;
using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.UwpApp.Activation;
using Inventory.UwpApp.Services;
using Inventory.UwpApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.UwpApp
{
    public sealed partial class App : Application
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
                .AddSingleton<ProductServiceUwp>()
                .AddSingleton<CustomerServiceUwp>()
                .AddSingleton<OrderServiceUwp>()
                //.AddSingleton<OrderItemServiceUwp>()

                ////.AddSingleton<IMessageService, MessageService>()
                .AddSingleton<FilePickerService, FilePickerService>()

                // Views and ViewModels
                //.AddTransient<ShellView>()
                .AddTransient<ShellViewModel>()
                .AddTransient<MainViewModel>()
                .AddTransient<ContentGridDetailViewModel>()
                .AddTransient<ContentGridViewModel>()
                .AddTransient<DataGridViewModel>()
                .AddTransient<ListDetailsViewModel>()
                .AddTransient<SettingsViewModel>()
                .AddTransient<TreeViewViewModel>()
                .AddTransient<TwoPaneViewViewModel>()

                .AddTransient<DashboardViewModel>()
                //.AddTransient<SettingsViewModel>()
                //.AddTransient<ValidateConnectionViewModel>()
                //.AddTransient<CreateDatabaseViewModel>()

                .AddTransient<CustomerListViewModel>()
                .AddTransient<CustomerDetailsViewModel>()
                .AddTransient<CustomersViewModel>()

                //.AddTransient<ProductListViewModel>()
                //.AddTransient<ProductDetailsViewModel>()
                //.AddTransient<ProductsViewModel>()

                //.AddTransient<OrderDetailsViewModel>()
                //.AddTransient<OrderDetailsWithItemsViewModel>()
                .AddTransient<OrderListViewModel>()
                .AddTransient<OrdersViewModel>()

                //.AddTransient<OrderItemListViewModel>()
                //.AddTransient<OrderItemDetailsViewModel>()
                //.AddTransient<OrderItemsViewModel>()

                //.AddTransient<LogsViewModel>()
                //.AddTransient<LogListViewModel>()
                //.AddTransient<LogDetailsViewModel>()

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
