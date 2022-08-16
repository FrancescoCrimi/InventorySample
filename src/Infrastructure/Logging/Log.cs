using System;

namespace CiccioSoft.Inventory.Infrastructure.Logging
{
    public enum LogType
    {
        Information,
        Warning,
        Error
    }

    public class Log
    {
        public Log() { }
        public virtual int Id { get; set; }
        public virtual bool IsRead { get; set; }
        public virtual string Name { get; set; }
        public DateTimeOffset DateTime { get; set; }        
        public virtual string User { get; set; }
        public LogType Type { get; set; }
        public string Source { get; set; }
        public string Action { get; set; }
        public virtual string Message { get; set; }
        public string Description { get; set; }

        //public virtual string Logger { get; set; }
        //public virtual string Exception { get; set; }
    }
}
