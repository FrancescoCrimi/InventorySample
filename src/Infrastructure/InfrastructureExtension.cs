using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiccioSoft.Inventory.Infrastructure
{
    public static class InfrastructureExtension
    {
        static IAppSettings settings;

        public static IServiceCollection AddInventoryInfrastructure(this IServiceCollection serviceCollection)
        {
            settings = serviceCollection.BuildServiceProvider().GetService<IAppSettings>();

            serviceCollection

                .AddLogging(AddLogging);

            return serviceCollection;
        }

        private static void AddLogging(ILoggingBuilder loggingBuilder)
        {
            //loggingBuilder.ClearProviders();
            //loggingBuilder.AddConfiguration();

            // Add visual studio viewer
            //loggingBuilder.AddDebug();

            // Add NLog
            loggingBuilder.AddNLog(Logging.ConfigureNLog(settings));
        }

    }
}
