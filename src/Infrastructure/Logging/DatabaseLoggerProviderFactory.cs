using Inventory.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Logging
{
    public class DatabaseLoggerProviderFactory : ILoggerProviderFactory
    {
        private readonly ISettingsService _settingsService;

        public DatabaseLoggerProviderFactory(ISettingsService settingsService )
        {
            _settingsService = settingsService;
        }

        public ILoggerProvider CreateLoggerProvider()
        {
            throw new NotImplementedException();
        }

        public LogLevel GetMinimumLevel()
        {
            throw new NotImplementedException();
        }
    }
}
