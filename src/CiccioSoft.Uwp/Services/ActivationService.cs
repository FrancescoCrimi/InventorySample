using CiccioSoft.Inventory.Uwp.Activation;
using CiccioSoft.Inventory.Uwp.Services.Infrastructure;
using CiccioSoft.Inventory.Uwp.ViewModels;
using CiccioSoft.Inventory.Uwp.Views;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CiccioSoft.Inventory.Uwp.Services
{
    // For more information on understanding and extending activation flow see
    // https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/activation.md
    internal class ActivationService
    {
        private object _lastActivationArgs;

        public ActivationService()
        {
        }

        public async Task ActivateAsync(object activationArgs)
        {
            if (IsInteractive(activationArgs))
            {
                // Initialize services that you need before app activation
                // take into account that the splash screen is shown while this code runs.
                await InitializeAsync();

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (Window.Current.Content == null)
                {
                    // Create a Shell or Frame to act as the navigation context
                    var _shell = Ioc.Default.GetService<ShellView>();
                    Window.Current.Content = _shell ?? (UIElement)new Frame();
                }
            }

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(activationArgs);
            _lastActivationArgs = activationArgs;

            if (IsInteractive(activationArgs))
            {
                // Ensure the current window is active
                Window.Current.Activate();

                // Tasks after activation
                await StartupAsync();
            }
        }

        private async Task InitializeAsync()
        {
            //await ThemeSelectorService.InitializeAsync().ConfigureAwait(false);
            //await WindowManagerService.Current.InitializeAsync();

            ConfigureNavigation();
            await EnsureLogDbAsync();
            await EnsureDatabaseAsync();
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = GetActivationHandlers()
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (IsInteractive(activationArgs))
            {
                //var defaultHandler = new DefaultActivationHandler();
                var defaultHandler = Ioc.Default.GetService<DefaultActivationHandler>();
                if (defaultHandler.CanHandle(activationArgs))
                {
                    await defaultHandler.HandleAsync(activationArgs);
                }
            }
        }

        private async Task StartupAsync()
        {
            //await ThemeSelectorService.SetRequestedThemeAsync();
            await ConfigureLookupTables();
            var logger = Ioc.Default.GetRequiredService<ILogger<ActivationService>>();
            //await logService.WriteAsync(Data.LogType.Information, "Startup", "Configuration", "Application Start", $"Application started.");
            //logger.LogInformation($"Application started.");
        }

        private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            yield break;
        }

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }


        private void ConfigureNavigation()
        {
            var pageService = Ioc.Default.GetService<PageService>();

            //pageService.Register<ShellViewModel, ShellView>();

            pageService.Register<DashboardViewModel, DashboardView>();

            pageService.Register<CustomersViewModel, CustomersView>();
            pageService.Register<CustomerDetailsViewModel, CustomerView>();

            pageService.Register<OrdersViewModel, OrdersView>();
            pageService.Register<OrderDetailsViewModel, OrderView>();

            pageService.Register<OrderItemsViewModel, OrderItemsView>();
            pageService.Register<OrderItemDetailsViewModel, OrderItemView>();

            pageService.Register<ProductsViewModel, ProductsView>();
            pageService.Register<ProductDetailsViewModel, ProductView>();

            pageService.Register<LogsViewModel, LogsView>();

            pageService.Register<SettingsViewModel, SettingsView>();
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

        private void CreateSqliteLogDb()
        {
            //string cn = configuration.GetConnectionString("SqliteLogConnection");
            string cn = AppSettings.Current.LogConnectionString;
            string createtablestr =
@"CREATE TABLE Log (
Id INTEGER PRIMARY KEY AUTOINCREMENT,
MachineName TEXT NOT NULL,
Logged TEXT NOT NULL,
Level TEXT NOT NULL,
Message TEXT NOT NULL,
Logger TEXT NULL,
Callsite TEXT NULL,
Exception TEXT NULL,
IsRead INTEGER NOT NULL
);";
            using (var conn = new SqliteConnection(cn))
            {
                conn.Open();
                var cmd = new SqliteCommand("DROP TABLE IF EXISTS Log", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = new SqliteCommand(createtablestr, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
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

        private async Task ConfigureLookupTables()
        {
            var lookupTables = Ioc.Default.GetService<LookupTableServiceFacade>();
            await lookupTables.InitializeAsync();
            //LookupTablesProxy.Instance = lookupTables;
        }
    }
}
