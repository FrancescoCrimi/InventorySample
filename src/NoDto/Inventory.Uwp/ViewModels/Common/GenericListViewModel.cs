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

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Uwp.Common;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.ViewModels.Message;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.Common
{
    public abstract class GenericListViewModel<TModel>
        : ViewModelBase where TModel : Infrastructure.Common.ObservableObject<TModel>
    {
        private int _itemsCount = 0;
        private string _query = null;
        private bool _isMultipleSelection = false;
        private ListToolbarMode _toolbarMode = ListToolbarMode.Default;
        private TModel _selectedItem = default;
        private IList<TModel> _items = null;
        private RelayCommand _newCommand;
        private RelayCommand _refreshCommand;
        private RelayCommand _startSelectionCommand;
        private RelayCommand _cancelSelectionCommand;
        private RelayCommand<IList<object>> _selectItemsCommand;
        private RelayCommand<IList<object>> _deselectItemsCommand;
        private RelayCommand<IndexRange[]> _selectRangesCommand;
        private RelayCommand _deleteSelectionCommand;

        public GenericListViewModel()
            : base()
        {
        }

        #region property

        public override string Title => string.IsNullOrEmpty(Query) ? $" ({ItemsCount})" : $" ({ItemsCount} for \"{Query}\")";

        public int ItemsCount
        {
            get => _itemsCount;
            set => SetProperty(ref _itemsCount, value);
        }

        public string Query
        {
            get => _query;
            set => SetProperty(ref _query, value);
        }

        public bool IsMultipleSelection
        {
            get => _isMultipleSelection;
            set => SetProperty(ref _isMultipleSelection, value);
        }

        public ListToolbarMode ToolbarMode
        {
            get => _toolbarMode;
            set => SetProperty(ref _toolbarMode, value);
        }

        public TModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    if (!IsMultipleSelection)
                    {
                        // fix _selectedItem == null
                        if (_selectedItem != null)
                        {
                            //MessageService.Send(this, "ItemSelected", _selectedItem);
                            Messenger.Send(new ViewModelsMessage<TModel>("ItemSelected", _selectedItem.Id));
                        }
                    }
                }
            }
        }

        public IList<TModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public List<TModel> SelectedItems { get; protected set; }

        public IndexRange[] SelectedIndexRanges { get; protected set; }

        #endregion


        #region icommand property

        public ICommand NewCommand => _newCommand
            ?? (_newCommand = new RelayCommand(OnNew));
        protected abstract void OnNew();

        public ICommand RefreshCommand => _refreshCommand
            ?? (_refreshCommand = new RelayCommand(OnRefresh));
        protected abstract void OnRefresh();

        public ICommand StartSelectionCommand => _startSelectionCommand
            ?? (_startSelectionCommand = new RelayCommand(OnStartSelection));
        protected virtual void OnStartSelection()
        {
            StatusMessage("Start selection");
            SelectedItem = null;
            SelectedItems = new List<TModel>();
            SelectedIndexRanges = null;
            IsMultipleSelection = true;
        }

        public ICommand CancelSelectionCommand => _cancelSelectionCommand
            ?? (_cancelSelectionCommand = new RelayCommand(OnCancelSelection));
        protected virtual void OnCancelSelection()
        {
            StatusReady();
            SelectedItems = null;
            SelectedIndexRanges = null;
            IsMultipleSelection = false;
            SelectedItem = Items?.FirstOrDefault();
        }

        public ICommand SelectItemsCommand => _selectItemsCommand
            ?? (_selectItemsCommand = new RelayCommand<IList<object>>(OnSelectItems));
        protected virtual void OnSelectItems(IList<object> items)
        {
            StatusReady();
            if (IsMultipleSelection)
            {
                SelectedItems.AddRange(items.Cast<TModel>());
                StatusMessage($"{SelectedItems.Count} items selected");
            }
        }

        public ICommand DeselectItemsCommand => _deselectItemsCommand
            ?? (_deselectItemsCommand = new RelayCommand<IList<object>>(OnDeselectItems));
        protected virtual void OnDeselectItems(IList<object> items)
        {
            if (items?.Count > 0)
            {
                StatusReady();
            }
            if (IsMultipleSelection)
            {
                foreach (TModel item in items)
                {
                    SelectedItems.Remove(item);
                }
                StatusMessage($"{SelectedItems.Count} items selected");
            }
        }

        public ICommand SelectRangesCommand => _selectRangesCommand
            ?? (_selectRangesCommand = new RelayCommand<IndexRange[]>(OnSelectRanges));
        protected virtual void OnSelectRanges(IndexRange[] indexRanges)
        {
            SelectedIndexRanges = indexRanges;
            var count = SelectedIndexRanges?.Sum(r => r.Length) ?? 0;
            StatusMessage($"{count} items selected");
        }

        public ICommand DeleteSelectionCommand => _deleteSelectionCommand
            ?? (_deleteSelectionCommand = new RelayCommand(OnDeleteSelection));
        protected abstract void OnDeleteSelection();

        #endregion
    }
}
