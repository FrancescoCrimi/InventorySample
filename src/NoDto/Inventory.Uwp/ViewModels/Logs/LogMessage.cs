// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

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
