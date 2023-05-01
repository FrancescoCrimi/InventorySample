using CommunityToolkit.Mvvm.Messaging.Messages;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Library.Common;
using System.Collections.Generic;

namespace Inventory.Uwp.ViewModels.Message
{
    public class ViewModelsMessage<T> : ValueChangedMessage<string> where T : ObservableDto, new()
    {
        public ViewModelsMessage(string value, long id)
            : base(value) => Id = id;
        public ViewModelsMessage(string value, IList<T> selectedItems)
            : base(value) => SelectedItems = selectedItems;
        public ViewModelsMessage(string value, IndexRange[] selectedIndexRanges)
            : base(value) => SelectedIndexRanges = selectedIndexRanges;
        public long Id { get; }
        public IList<T> SelectedItems { get; }
        public IndexRange[] SelectedIndexRanges { get; }
    }
}
