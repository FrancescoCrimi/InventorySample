using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Inventory.Domain.Model;
using Inventory.Uwp.Library.Common;

namespace Inventory.Uwp.ViewModels.Message
{
    internal class CustomerChangedMessage : ValueChangedMessage<string>
    {
        private readonly long _id;
        private readonly IndexRange[] _selectedIndexRanges;
        private readonly IList<Customer> _selectedItems;

        public CustomerChangedMessage(string value) : base(value) { }

        public CustomerChangedMessage(string value, long id) : base(value)
        {
            _id = id;
        }

        public CustomerChangedMessage(string value, IndexRange[] selectedIndexRanges) : base(value)
        {
            _selectedIndexRanges = selectedIndexRanges;
        }

        public CustomerChangedMessage(string value, IList<Customer> selectedItems) : base(value)
        {
            _selectedItems = selectedItems;
        }

        public long Id => _id;
        public IList<Customer> SelectedItems => _selectedItems;
        public IndexRange[] SelectedIndexRanges => _selectedIndexRanges;
    }
}
