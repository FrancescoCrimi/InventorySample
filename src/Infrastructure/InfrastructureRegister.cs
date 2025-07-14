using Inventory.Infrastructure.Logging;
using Inventory.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure
{
    public static class InfrastructureRegister
    {
        public static void Register(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IDatabaseConfigurationService, DatabaseConfigurationService>()
                .AddSingleton<LogDbContextFactory>()
                .AddSingleton<LogService>()
                .AddLogging(loggingBuilder => loggingBuilder
                    .ClearProviders()
                    .AddDebug()
                    //.AddConsole()
                    //.AddProvider(new CustomLoggerProvider(serviceCollection.BuildServiceProvider()))
                    .AddFilter("", LogLevel.Information)
                    .AddFilter<DatabaseLoggerProvider>("Microsoft.EntityFrameworkCore", LogLevel.None))
                .TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DatabaseLoggerProvider>());
        }
    }
}
