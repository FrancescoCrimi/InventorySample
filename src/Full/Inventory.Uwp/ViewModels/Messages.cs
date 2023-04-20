using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Inventory.Uwp.ViewModels
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
