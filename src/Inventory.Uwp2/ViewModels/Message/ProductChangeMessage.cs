using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Inventory.Domain.Model;
using Inventory.Uwp.Library.Common;

namespace Inventory.Uwp.ViewModels.Message
{
    public class ProductChangeMessage : ValueChangedMessage<string>
    {
        private readonly long _id;
        private readonly IndexRange[] _selectedIndexRanges;
        private readonly IList<Product> _selectedItems;

        public ProductChangeMessage(string value) : base(value) { }

        public ProductChangeMessage(string value, long id) : base(value)
        {
            _id = id;
        }

        public ProductChangeMessage(string value, IndexRange[] selectedIndexRanges) : base(value)
        {
            _selectedIndexRanges = selectedIndexRanges;
        }

        public ProductChangeMessage(string value, IList<Product> selectedItems) : base(value)
        {
            _selectedItems = selectedItems;
        }

        public long Id => _id;
        public IList<Product> SelectedItems => _selectedItems;
        public IndexRange[] SelectedIndexRanges => _selectedIndexRanges;
    }
}
