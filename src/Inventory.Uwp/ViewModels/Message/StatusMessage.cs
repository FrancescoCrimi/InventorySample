using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Inventory.Uwp.ViewModels
{
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
