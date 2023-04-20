using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Inventory.Domain.Model;
using Inventory.Uwp.Library.Common;

namespace Inventory.Uwp.ViewModels.Message
{
    internal class OrderItemChangeMessage : ValueChangedMessage<string>
    {
        private readonly long _id;
        private readonly IList<OrderItem> _selectedItems;
        private readonly IndexRange[] _selectedIndexRanges;
        private readonly long _orderID;
        private readonly int _orderLine;

        public OrderItemChangeMessage(string value) : base(value)
        {
        }

        public OrderItemChangeMessage(string value, long id) : base(value)
        {
            _id = id;
        }

        public OrderItemChangeMessage(string value, IList<OrderItem> selectedItems) : base(value)
        {
            _selectedItems = selectedItems;
        }

        public OrderItemChangeMessage(string value, IndexRange[] selectedIndexRanges) : base(value)
        {
            _selectedIndexRanges = selectedIndexRanges;
        }

        public OrderItemChangeMessage(string value, long orderID, int orderLine) : base(value)
        {
            _orderID = orderID;
            _orderLine = orderLine;
        }

        public long Id => _id;
        public IList<OrderItem> SelectedItems => _selectedItems;
        public IndexRange[] SelectedIndexRanges => _selectedIndexRanges;
        public long OrderID => _orderID;
        public int OrderLine => _orderLine;
    }
}
