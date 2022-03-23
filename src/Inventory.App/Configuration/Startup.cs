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

using CiccioSoft.Inventory.Data.DataContexts;
using CiccioSoft.Inventory.Uwp.Services;
using CiccioSoft.Inventory.Uwp.ViewModels;
using CiccioSoft.Inventory.Uwp.Views;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using NLog.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace CiccioSoft.Inventory.Uwp
{
    public class Startup
    {
        public async Task ConfigureAsync()
        {
            Ioc.Default.ConfigureServices(ConfigureServices());
            ConfigureNavigation();
            await EnsureLogDbAsync();
            await EnsureDatabaseAsync();
            await ConfigureLookupTables();

            var logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(Startup).Name);
            logger.LogInformation("Application started.");

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddLogging(AddLogging);
            AddDbContexts(services);
            AddServices(services);
            return services.BuildServiceProvider();
        }

        private void AddLogging(ILoggingBuilder loggingBuilder)
        {
            //loggingBuilder.ClearProviders();
            //loggingBuilder.AddConfiguration();

            // Add visual studio viewer
            //loggingBuilder.AddDebug();

            // Add NLog
            loggingBuilder.AddNLog(ConfigureNLog());
        }

        private void AddDbContexts(IServiceCollection services)
        {
            services
               .AddDbContext<LogDbContext>(option =>
               {
                   option.UseSqlite(AppSettings.Current.AppLogConnectionString);
               });
        }

        private void AddServices(ServiceCollection services)
        {
            services
            .AddSingleton<ISettingsService, SettingsService>()
            .AddSingleton<IDataServiceFactory, DataServiceFactory>()
            .AddSingleton<ILookupTables, LookupTables>()
            .AddSingleton<ICustomerService, CustomerService>()
            .AddSingleton<IOrderService, OrderService>()
            .AddSingleton<IOrderItemService, OrderItemService>()
            .AddSingleton<IProductService, ProductService>()

            //.AddSingleton<IMessageService, MessageService>()
            .AddSingleton<ILogService, LogService>()
            .AddSingleton<IDialogService, DialogService>()
            .AddSingleton<IFilePickerService, FilePickerService>()
            //.AddSingleton<ILoginService, LoginService>()

            //.AddScoped<IContextService, ContextService>()
            .AddScoped<INavigationService, NavigationService>()
            .AddSingleton<PageService>()
            .AddSingleton<IWindowService, WindowService>()
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
        }

        private NLog.Config.LoggingConfiguration ConfigureNLog()
        {
            var config = new NLog.Config.LoggingConfiguration();

            ////var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            //var logconsole = new NLog.Targets.ColoredConsoleTarget("logconsole");
            //config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logconsole);

            var vsDebug = new NLog.Targets.DebuggerTarget();
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, vsDebug);

            var db = new NLog.Targets.DatabaseTarget("database");
            db.DBProvider = "Microsoft.Data.Sqlite.SqliteConnection, Microsoft.Data.Sqlite";
            db.ConnectionString = AppSettings.Current.LogConnectionString;
            db.CommandText =
                @"insert into Log (
                MachineName, Logged, Level, Message,
                Logger, Callsite, Exception
                ) values(
                @MachineName, @Logged, @Level, @Message,
                @Logger, @Callsite, @Exception
                );";
            db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@MachineName", NLog.Layouts.Layout.FromString("${machinename}")));
            db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Logged", NLog.Layouts.Layout.FromString("${date}")));
            db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Level", NLog.Layouts.Layout.FromString("${level}")));
            db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Message", NLog.Layouts.Layout.FromString("${message}")));
            db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Logger", NLog.Layouts.Layout.FromString("${logger}")));
            db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Callsite", NLog.Layouts.Layout.FromString("${callsite}")));
            db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Exception", NLog.Layouts.Layout.FromString("${exception:tostring}")));
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, db);
            return config;
        }



        private void ConfigureNavigation()
        {
            var pageService = Ioc.Default.GetService<PageService>();
            pageService.Register<LoginViewModel, LoginView>();

            pageService.Register<ShellViewModel, ShellView>();
            pageService.Register<MainShellViewModel, MainShellView>();

            pageService.Register<DashboardViewModel, DashboardView>();

            pageService.Register<CustomersViewModel, CustomersView>();
            pageService.Register<CustomerDetailsViewModel, CustomerView>();

            pageService.Register<OrdersViewModel, OrdersView>();
            pageService.Register<OrderDetailsViewModel, OrderView>();

            pageService.Register<OrderItemsViewModel, OrderItemsView>();
            pageService.Register<OrderItemDetailsViewModel, OrderItemView>();

            pageService.Register<ProductsViewModel, ProductsView>();
            pageService.Register<ProductDetailsViewModel, ProductView>();

            pageService.Register<AppLogsViewModel, AppLogsView>();

            pageService.Register<SettingsViewModel, SettingsView>();
        }

        private async Task EnsureLogDbAsync()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var appLogFolder = await localFolder.CreateFolderAsync(AppSettings.AppLogPath, CreationCollisionOption.OpenIfExists);
            if (await appLogFolder.TryGetItemAsync(AppSettings.LogName) == null)
            {
                //var sourceLogFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/AppLog/AppLog.db"));
                //var targetLogFile = await appLogFolder.CreateFileAsync(AppSettings.AppLogName, CreationCollisionOption.ReplaceExisting);
                //await sourceLogFile.CopyAndReplaceAsync(targetLogFile);
                CreateSqliteLogDb();
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
            var lookupTables = Ioc.Default.GetService<ILookupTables>();
            await lookupTables.InitializeAsync();
            LookupTablesProxy.Instance = lookupTables;
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
Exception TEXT NULL
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
    }
}
