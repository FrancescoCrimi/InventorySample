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
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection;
using Inventory.ViewModels;
using Inventory.Data.DataContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using NLog.Extensions.Logging;
using Inventory.Views;
using Windows.System;

namespace Inventory
{
    sealed partial class App : Application
    {
        //private readonly ILogger logger;

        public App()
        {
            InitializeComponent();

            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;

            Ioc.Default.ConfigureServices(ConfigureServices());
            //logger = Ioc.Default.GetRequiredService<ILogger<App>>();
            //var factory = Ioc.Default.GetService<ILoggerFactory>();
            //logger = factory.CreateLogger(typeof(App).Name);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                Frame rootFrame = new Frame();
                await Startup.ConfigureAsync();

                var shellArgs = new ShellArgs
                {
                    ViewModel = typeof(DashboardViewModel),
                    Parameter = null,
                    UserInfo = await TryGetUserInfoAsync(e as IActivatedEventArgsWithUser)
                };

                rootFrame.Navigate(typeof(MainShellView), shellArgs);

                Window.Current.Content = rootFrame;
                //Window.Current.Activate();
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

        private async Task<UserInfo> TryGetUserInfoAsync(IActivatedEventArgsWithUser argsWithUser)
        {
            if (argsWithUser != null)
            {
                var user = argsWithUser.User;
                var userInfo = new UserInfo
                {
                    AccountName = await user.GetPropertyAsync(KnownUserProperties.AccountName) as String,
                    FirstName = await user.GetPropertyAsync(KnownUserProperties.FirstName) as String,
                    LastName = await user.GetPropertyAsync(KnownUserProperties.LastName) as String
                };
                if (!userInfo.IsEmpty)
                {
                    if (String.IsNullOrEmpty(userInfo.AccountName))
                    {
                        userInfo.AccountName = $"{userInfo.FirstName} {userInfo.LastName}";
                    }
                    var pictureStream = await user.GetPictureAsync(UserPictureSize.Size64x64);
                    if (pictureStream != null)
                    {
                        userInfo.PictureSource = await BitmapTools.LoadBitmapAsync(pictureStream);
                    }
                    return userInfo;
                }
            }
            return UserInfo.Default;
        }

        private static NLog.Config.LoggingConfiguration ConfigureNLog()
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
    }
}
