using Inventory.Data;
using Inventory.Data.DataContexts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.Infrastructure.LogService
{
    public class LoggerImpl : ILogger
    {
        private readonly AppLogDbContext dbContext;
        private readonly string CategoryName;
        private readonly string _logPrefix;

        public LoggerImpl(string categoryName,
                          string logPrefix,
                          AppLogDbContext dbContext)
        {
            CategoryName = categoryName;
            _logPrefix = logPrefix;
            this.dbContext = dbContext;
            //MessageService = messageService;
        }

        //public IMessageService MessageService { get; }

        public IDisposable BeginScope<TState>(TState state) => default;
        //public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;


        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel,
                                EventId eventId,
                                TState state,
                                Exception exception,
                                Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            string message = _logPrefix;
            if (formatter != null)
            {
                message += formatter(state, exception);
            }

            // Implement log writter as you want. I am using Console
            Console.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {CategoryName} - {message}");
            System.Diagnostics.Debug.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {CategoryName} - {message}");


        }



        public async Task WriteAsync(LogType type, string source, string action, Exception ex)
        {
            await WriteAsync(LogLevel.Error, source, action, ex.Message, ex.ToString());
            Exception deepException = ex.InnerException;
            while (deepException != null)
            {
                await WriteAsync(LogLevel.Error, source, action, deepException.Message, deepException.ToString());
                deepException = deepException.InnerException;
            }
        }

        public async Task WriteAsync(LogLevel type, string source, string action, string message, string description)
        {
            var appLog = new AppLog()
            {
                User = AppSettings.Current.UserName ?? "App",
                Type = type,
                Source = source,
                Action = action,
                Message = message,
                Description = description,
            };

            appLog.IsRead = type != LogLevel.Error;
            appLog.DateTime = DateTime.UtcNow;
            //Entry(appLog).State = EntityState.Added;
            await dbContext.Logs.AddAsync(appLog);
            await dbContext.SaveChangesAsync();

            //MessageService.Send(this, "LogAdded", appLog);
        }

    }
}
