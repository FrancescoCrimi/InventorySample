using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace CiccioSoft.Inventory.Infrastructure.Logging
{
    public class CustomLogger : ILogger
    {
        private readonly string categoryName;
        private readonly IServiceProvider serviceProvider;

        public CustomLogger(string categoryName, IServiceProvider serviceProvider)
        {
            this.categoryName = categoryName;
            this.serviceProvider = serviceProvider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            //return default!;
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogDbContext logDbContext = serviceProvider.GetService<LogDbContext>();

            string message = "";
            if (formatter != null)
            {
                message += formatter(state, exception);
            }
            // Implement log writter as you want. I am using Console
            Console.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {categoryName} - {message}");
            System.Diagnostics.Debug.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {categoryName} - {message}");

            var appLog = new Log()
            {
                //User = AppSettings.Current.UserName ?? "App",
                User = "App",
                //Type = type,
                Source = categoryName,
                Action = "action",
                Message = message,
                DateTime = DateTime.Now,
                IsRead = false
                //MachineName = "Suca",
                //Level = "Suca"
                //Description = description,
            };

            logDbContext.Logs.Add(appLog);
            logDbContext.SaveChanges();

            LogService.RaiseNewEventLog();
        }
    }
}
