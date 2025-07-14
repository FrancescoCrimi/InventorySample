using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Logging
{
    public interface ILoggerProviderFactory
    {
        ILoggerProvider CreateLoggerProvider();
        LogLevel GetMinimumLevel();
    }
}
