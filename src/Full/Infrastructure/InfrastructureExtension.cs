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
                    .AddDatabaseLogger(serviceCollection)
                    .AddFilter("", LogLevel.Information));
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
