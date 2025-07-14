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

using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly LogDbContext _logDbContext;

        public DatabaseLogger(string categoryName,
                              LogDbContext logDbContext)
        {
            _categoryName = categoryName;
            _logDbContext = logDbContext;
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
                var message = "";
                if (formatter != null)
                {
                    message += formatter(state, exception);
                }

                //// Implement log writter as you want. I am using Console
                //Console.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {categoryName} - {message}");
                //System.Diagnostics.Debug.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {categoryName} - {message}");

                // Use EF core
                var appLog = new Log()
                {
                    //User = AppSettings.Current.UserName ?? "App",
                    User = "App",
                    DateTime = DateTime.Now,
                    Level = logLevel,
                    Source = _categoryName,
                    Action = eventId.Name,
                    Message = message + exception?.Message,
                    Description = exception?.ToString(),
                    IsRead = false,
                };

                _logDbContext.Logs.Add(appLog);
                await _logDbContext.SaveChangesAsync();
            });

            // Todo: provare ad inserire la chiamata al metodo per scatenare l'evento dentro il task
            LogService.RaiseNewEventLog();
        }
    }
}
