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

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Logging
{
    public class CustomLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IServiceProvider _serviceProvider;

        public CustomLogger(string categoryName,
                            IServiceProvider serviceProvider)
        {
            _categoryName = categoryName;
            _serviceProvider = serviceProvider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Task.Run(async () =>
            {
                var logDbContext = _serviceProvider.GetService<LogDbContext>();

                var message = "";
                if (formatter != null)
                {
                    message += formatter(state, exception);
                }

                //// Implement log writter as you want. I am using Console
                //Console.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {categoryName} - {message}");
                //System.Diagnostics.Debug.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {categoryName} - {message}");

                var appLog = new Log()
                {
                    //User = AppSettings.Current.UserName ?? "App",
                    User = "App",
                    DateTime = DateTime.Now,
                    Level = logLevel,
                    Source = _categoryName,
                    Action = eventId.Name,
                    Message = message + exception?.Message,
                    Description = exception?.ToString() ?? string.Empty,
                    IsRead = false,
                };

                logDbContext.Logs.Add(appLog);
                await logDbContext.SaveChangesAsync();
            });

            // Todo: provare ad inserire la chiamata al metodo per scatenare l'evento dentro il task
            LogService.RaiseNewEventLog();
        }
    }
}
