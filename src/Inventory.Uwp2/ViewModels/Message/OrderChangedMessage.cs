using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Inventory.Domain.Model;
using Inventory.Uwp.Library.Common;

namespace Inventory.Uwp.ViewModels.Message
{
    public class OrderChangedMessage : ValueChangedMessage<string>
    {
        private readonly long _id;
        private readonly IndexRange[] _selectedIndexRanges;
        private readonly IList<Order> _selectedItems;

        public OrderChangedMessage(string value) : base(value) { }

        public OrderChangedMessage(string value, long id) : base(value)
        {
            _id = id;
        }

        public OrderChangedMessage(string value, IndexRange[] selectedIndexRanges) : base(value)
        {
            _selectedIndexRanges = selectedIndexRanges;
        }

        public OrderChangedMessage(string value, IList<Order> selectedItems) : base(value)
        {
            _selectedItems = selectedItems;
        }

        public long Id => _id;
        public IList<Order> SelectedItems => _selectedItems;
        public IndexRange[] SelectedIndexRanges => _selectedIndexRanges;
    }
}
