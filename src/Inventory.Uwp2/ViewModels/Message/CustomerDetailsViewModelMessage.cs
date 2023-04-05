using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Inventory.Uwp.ViewModels.Message
{
    internal class CustomerDetailsViewModelMessage : ValueChangedMessage<string>
    {
        public CustomerDetailsViewModelMessage(string value) : base(value)
        {
        }
    }
}
