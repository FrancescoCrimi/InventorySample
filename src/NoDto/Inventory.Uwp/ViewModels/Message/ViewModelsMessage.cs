using CommunityToolkit.Mvvm.Messaging.Messages;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Library.Common;
using System.Collections.Generic;

namespace Inventory.Uwp.ViewModels.Message
{
    public class ViewModelsMessage<T> : ValueChangedMessage<string> where T : ObservableObject<T>
    {
        private readonly long _id;
        private readonly IList<T> _selectedItems;
        private readonly IndexRange[] _selectedIndexRanges;

        public ViewModelsMessage(string value, long id) : base(value) => _id = id;
        public ViewModelsMessage(string value, IList<T> selectedItems) : base(value) => _selectedItems = selectedItems;
        public ViewModelsMessage(string value, IndexRange[] selectedIndexRanges) : base(value) => _selectedIndexRanges = selectedIndexRanges;

        public long Id => _id;
        public IList<T> SelectedItems => _selectedItems;
        public IndexRange[] SelectedIndexRanges => _selectedIndexRanges;
    }
}
