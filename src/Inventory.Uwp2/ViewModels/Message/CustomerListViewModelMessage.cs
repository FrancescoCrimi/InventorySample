using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Inventory.Uwp.ViewModels.Message
{
    public class CustomerListViewModelMessage : ValueChangedMessage<string>
    {
        public CustomerListViewModelMessage(string value) : base(value)
        {
        }
    }
}
