using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

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
