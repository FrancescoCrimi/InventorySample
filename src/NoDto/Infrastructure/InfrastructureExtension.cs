// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

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
                    .AddFilter("", LogLevel.Warning)
                );
        }

        private static ILoggingBuilder AddSqliteDatabase(this ILoggingBuilder loggingBuilder, IServiceCollection serviceCollection)
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
