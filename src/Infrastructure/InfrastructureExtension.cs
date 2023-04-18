using Inventory.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets;
using System;

namespace Inventory.Infrastructure
{
    public static class InfrastructureExtension
    {
        private static IAppSettings settings;

        public static IServiceCollection AddInventoryInfrastructure(this IServiceCollection serviceCollection)
        {
            settings = serviceCollection.BuildServiceProvider().GetService<IAppSettings>();

            serviceCollection

                // Aggiungi DbContext per LogService
                .AddDbContext<LogDbContext>(option =>
                {
                    option.UseSqlite(settings.AppLogConnectionString);
                    //option.UseMySql(settings.LogMySqlConnectionString);
                    //option.UseSqlServer(AppSettings.Current.MsLogConnectionString);
                }, ServiceLifetime.Transient)

                // Aggiungi LogService
                .AddSingleton<LogService>()

                // Aggiungi Logging
                .AddLogging(AddLogging);

                //// Aggiungi Custom Logger            
                //.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, CustomLoggerProvider>());

            return serviceCollection;
        }

        private static void AddLogging(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.ClearProviders();
            //loggingBuilder.AddConfiguration();

            // Add visual studio viewer
            loggingBuilder.AddDebug();

            //loggingBuilder.AddConsole();

            //// Add NLog
            //loggingBuilder.AddNLog(ConfigureNLog(settings));

            //// Add filter
            //loggingBuilder.AddFilter("Microsoft.EntityFrameworkCore", Microsoft.Extensions.Logging.LogLevel.None);

        }

        internal static NLog.Config.LoggingConfiguration ConfigureNLog(IAppSettings settings)
        {
            var config = new NLog.Config.LoggingConfiguration();

            ////var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            //var logconsole = new NLog.Targets.ColoredConsoleTarget("logconsole");
            //config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logconsole);

            var vsDebug = new DebuggerTarget();
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, vsDebug);


            //// Database Target

            //var db = new NLog.Targets.DatabaseTarget("database");

            ////db.DBProvider = "Microsoft.Data.Sqlite.SqliteConnection, Microsoft.Data.Sqlite";
            //////db.IsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
            ////db.ConnectionString = AppSettings.Current.LogConnectionString;

            ////db.DBProvider = "Microsoft.Data.SqlClient.SqlConnection, Microsoft.Data.SqlClient";
            ////db.ConnectionString = AppSettings.Current.MsLogConnectionString;

            //db.DBProvider = "MySql.Data.MySqlClient.MySqlConnection, MySqlConnector";
            //db.ConnectionString = settings.LogMySqlConnectionString;

            //db.CommandText =
            //    @"insert into Log (
            //    MachineName, Logged, Level, Message,
            //    Logger, Callsite, Exception
            //    ) values(
            //    @MachineName, @Logged, @Level, @Message,
            //    @Logger, @Callsite, @Exception
            //    );";
            //db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@MachineName", NLog.Layouts.Layout.FromString("${machinename}")));
            //db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Logged", NLog.Layouts.Layout.FromString("${date}")));
            //db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Level", NLog.Layouts.Layout.FromString("${level}")));
            //db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Message", NLog.Layouts.Layout.FromString("${message}")));
            //db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Logger", NLog.Layouts.Layout.FromString("${logger}")));
            //db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Callsite", NLog.Layouts.Layout.FromString("${callsite}")));
            //db.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Exception", NLog.Layouts.Layout.FromString("${exception:tostring}")));

            //config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, new NLog.Targets.NullTarget(), "Microsoft.EntityFrameworkCore.*", true);
            //config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, db);

            //// Aggiungi un target -> method 
            var target = new MethodCallTarget("MethodTarget", Sucaaaaa);

            return config;
        }

        private static void Sucaaaaa(LogEventInfo arg1, object[] arg2)
        {
            AddLogEvent?.Invoke(null, new EventArgs());
        }

        public static event EventHandler AddLogEvent;
    }
}
