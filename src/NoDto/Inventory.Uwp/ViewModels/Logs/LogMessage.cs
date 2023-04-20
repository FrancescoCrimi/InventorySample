using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Library.Common;

namespace Inventory.Uwp.ViewModels.Logs
{
    internal class LogMessage : ValueChangedMessage<string>
    {
        public LogMessage(string value, Log log ) : base(value)
        {
            Log  = log;
        }
        public LogMessage(string value, IList<Log> selectedItems) : base(value)
        {
            SelectedItems = selectedItems;
        }
        public LogMessage(string value, IndexRange[] selectedIndexRanges) : base(value)
        {
            SelectedIndexRanges = selectedIndexRanges;
        }
        public LogMessage(string value, long id) : base(value)
        {
            Id = id;
        }
        public long Id { get; }
        public Log Log { get; }
        public IList<Log> SelectedItems { get; }
        public IndexRange[] SelectedIndexRanges { get; }
    }
}
