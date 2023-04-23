using Microsoft.Extensions.Logging;
using System;

namespace Inventory.Infrastructure.Logging
{
    public class Log
    {
        public Log()
        {
        }
        public int Id
        {
            get; set;
        }
        public bool IsRead
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public DateTimeOffset DateTime
        {
            get; set;
        }
        public string User
        {
            get; set;
        }
        public LogLevel Level
        {
            get; set;
        }
        public string Source
        {
            get; set;
        }
        public string Action
        {
            get; set;
        }
        public string Message
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }

        //public virtual string Logger { get; set; }
        //public virtual string Exception { get; set; }
    }
}
