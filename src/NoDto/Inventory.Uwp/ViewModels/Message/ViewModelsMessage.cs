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
