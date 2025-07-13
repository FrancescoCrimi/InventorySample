using Inventory.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure
{
    public static class InfrastructureRegister
    {
        private static IAppSettings settings;

        public static void Register(IServiceCollection serviceCollection)
        {
            // Register your infrastructure services here
            // For example, you might register logging, settings, or database providers
            // Example:
            // services.AddSingleton<ILogger, ConsoleLogger>();
            // services.AddSingleton<IAppSettings, AppSettings>();
            // You can also register other components if needed
            // services.AddSingleton<IDatabaseProvider, SqliteDatabaseProvider>();

            settings = serviceCollection.BuildServiceProvider().GetService<IAppSettings>();
             serviceCollection
                .AddLogging(loggingBuilder => loggingBuilder
                    .ClearProviders()
                    .AddDebug()
                    //.AddConsole()
                    .AddDatabaseLogger(serviceCollection)
                    .AddFilter("", LogLevel.Information));
            //return InfrastructurePersistenceRegister.Register(serviceCollection);

        }

        private static ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder loggingBuilder,
                                                         IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddDbContext<LogDbContext>(option =>
                {
                    option.UseSqlite(settings.AppLogConnectionString);
                    //option.EnableSensitiveDataLogging(true);
                }, ServiceLifetime.Transient)
                .AddSingleton<LogService>()
                .TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, CustomLoggerProvider>());

            return loggingBuilder
                .AddFilter<CustomLoggerProvider>("Microsoft.EntityFrameworkCore", LogLevel.None);
        }
    }
}
