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
                    .AddDatabaseLogger(serviceCollection));
        }

        private static ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder loggingBuilder,
                                                         IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddDbContext<LogDbContext>(option =>
                {
                    option.UseSqlite(settings.AppLogConnectionString);
                    option.EnableSensitiveDataLogging(true);
                }, ServiceLifetime.Transient)
                .AddSingleton<LogService>();
                //.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, CustomLoggerProvider>());

            return loggingBuilder

                // Add filter
                //.AddFilter("Microsoft.EntityFrameworkCore", Microsoft.Extensions.Logging.LogLevel.None)
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
