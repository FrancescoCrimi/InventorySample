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

    public class ItemMessage : ValueChangedMessage<string>
    {
        public ItemMessage(string value, long id = 0) : base(value)
        {
            Id = id;
        }
        public long Id
        {
            get;
        }
    }
}
