using Inventory.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure
{
    public static class InfrastructureExtension
    {
        private static IAppSettings settings;

        public static IServiceCollection AddInventoryInfrastructure(this IServiceCollection serviceCollection)
        {
            settings = serviceCollection.BuildServiceProvider().GetService<IAppSettings>();
            return serviceCollection
                .AddLogging(loggingBuilder => loggingBuilder
                    .ClearProviders()
                    .AddDebug()
                    //.AddConsole()
                    .AddSqliteDatabase(serviceCollection)
                );
        }

        private static ILoggingBuilder AddSqliteDatabase(this ILoggingBuilder loggingBuilder, IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddDbContext<LogDbContext>(option =>
                {
                    option.UseSqlite(settings.AppLogConnectionString);
                }, ServiceLifetime.Transient)
                .AddSingleton<LogService>()
                .TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, CustomLoggerProvider>());

            // Add filter
            return loggingBuilder
                //.AddFilter("Microsoft.EntityFrameworkCore", Microsoft.Extensions.Logging.LogLevel.None);
                .AddFilter((provider, category, logLevel) =>
                {
                    if (provider == typeof(CustomLoggerProvider).FullName
                    && category.Contains("Microsoft.EntityFrameworkCore")
                    && logLevel <= Microsoft.Extensions.Logging.LogLevel.Information)
                        return false;
                    return true;
                });

        }
    }
}
