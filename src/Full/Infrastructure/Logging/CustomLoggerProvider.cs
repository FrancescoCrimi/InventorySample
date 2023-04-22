using System;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Logging
{
    public class CustomLoggerProvider : ILoggerProvider
    {
        private readonly IServiceProvider serviceProvider;

        public CustomLoggerProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomLogger(categoryName, serviceProvider);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
