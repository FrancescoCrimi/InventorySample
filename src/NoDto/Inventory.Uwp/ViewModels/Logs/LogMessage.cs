using CommunityToolkit.Mvvm.Messaging.Messages;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Library.Common;
using System.Collections.Generic;

namespace Inventory.Uwp.ViewModels.Logs
{
    public class LogMessage : ValueChangedMessage<string>
    {
        public LogMessage(string value, long id) : base(value) => Id = id;
        public LogMessage(string value, IList<Log> selectedItems) : base(value) => SelectedItems = selectedItems;
        public LogMessage(string value, IndexRange[] selectedIndexRanges) : base(value) => SelectedIndexRanges = selectedIndexRanges;

        public long Id { get; }
        public IList<Log> SelectedItems { get; }
        public IndexRange[] SelectedIndexRanges { get; }
    }
}
