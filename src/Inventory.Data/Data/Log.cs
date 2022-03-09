using System;

namespace Inventory.Data
{
    public class Log
    {

        public Log()
        {
        }

        public virtual int Id { get; set; }

        public virtual string MachineName { get; set; }

        public virtual DateTime Logged { get; set; }

        public virtual string Level { get; set; }

        public virtual string Message { get; set; }

        public virtual string Logger { get; set; }

        public virtual string Callsite { get; set; }

        public virtual string Exception { get; set; }
    }
}
