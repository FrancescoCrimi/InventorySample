using CommunityToolkit.Mvvm.Messaging.Messages;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Library.Common;
using System.Collections.Generic;

namespace Inventory.Uwp.ViewModels
{
    public class LogMessage : ValueChangedMessage<string>
    {
        public LogMessage(string value, IndexRange[] indexRanges)
            : base(value) => IndexRanges = indexRanges;
        public LogMessage(string value, IList<Log> items)
            : base(value) => Items = items;
        public LogMessage(string value, long id)
            : base(value) => Id = id;
        public long Id { get; }
        public IList<Log> Items { get; }
        public IndexRange[] IndexRanges { get; }
    }
}
