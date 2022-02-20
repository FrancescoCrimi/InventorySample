using Inventory.Data.DataContexts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.Infrastructure.LogService
{
    public class DatabaseLoggerProvider : ILoggerProvider
    {
        private readonly IServiceProvider serviceProvider;

        public DatabaseLoggerProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ILogger CreateLogger(string categoryName)
        {
            var db = serviceProvider.GetService<AppLogDbContext>();
            var logger = new LoggerImpl(categoryName, "Suca: ", db);
            return logger;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
