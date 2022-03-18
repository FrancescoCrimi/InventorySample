using CiccioSoft.Inventory.Models;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.ViewModels
{
    public class ItemMessage<TItem> : ValueChangedMessage<TItem>
    {
        public ItemMessage(TItem value, string message)
            : base(value)
        {
            Message = message;
        }
        public string Message { get; }
    }

    public class StatusMessage : ValueChangedMessage<string>
    {
        public StatusMessage(string message, string args)
            : base(message)
        {
            Args = args;
        }
        public string Args { get; }
    }
}
