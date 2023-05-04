#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

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
    public abstract class GenericListViewModel<TModel> : ViewModelBase where TModel : Inventory.Infrastructure.Common.ObservableObject<TModel>
    {
        public GenericListViewModel()
            : base()
        {
        }

        //public LookupTableServiceFacade LookupTables => Ioc.Default.GetRequiredService<LookupTableServiceFacade>();

        public override string Title => string.IsNullOrEmpty(Query) ? $" ({ItemsCount})" : $" ({ItemsCount} for \"{Query}\")";

        private IList<TModel> _items = null;
        public IList<TModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private int _itemsCount = 0;
        public int ItemsCount
        {
            get => _itemsCount;
            set => SetProperty(ref _itemsCount, value);
        }

        private TModel _selectedItem = default;
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

        private string _query = null;
        public string Query
        {
            get => _query;
            set => SetProperty(ref _query, value);
        }

        private ListToolbarMode _toolbarMode = ListToolbarMode.Default;
        public ListToolbarMode ToolbarMode
        {
            get => _toolbarMode;
            set => SetProperty(ref _toolbarMode, value);
        }

        private bool _isMultipleSelection = false;
        public bool IsMultipleSelection
        {
            get => _isMultipleSelection;
            set => SetProperty(ref _isMultipleSelection, value);
        }

        public List<TModel> SelectedItems { get; protected set; }
        public IndexRange[] SelectedIndexRanges { get; protected set; }

        public ICommand NewCommand => new RelayCommand(OnNew);

        public ICommand RefreshCommand => new RelayCommand(OnRefresh);

        public ICommand StartSelectionCommand => new RelayCommand(OnStartSelection);
        protected virtual void OnStartSelection()
        {
            StatusMessage("Start selection");
            SelectedItem = null;
            SelectedItems = new List<TModel>();
            SelectedIndexRanges = null;
            IsMultipleSelection = true;
        }

        public ICommand CancelSelectionCommand => new RelayCommand(OnCancelSelection);
        protected virtual void OnCancelSelection()
        {
            StatusReady();
            SelectedItems = null;
            SelectedIndexRanges = null;
            IsMultipleSelection = false;
            SelectedItem = Items?.FirstOrDefault();
        }

        public ICommand SelectItemsCommand => new RelayCommand<IList<object>>(OnSelectItems);
        protected virtual void OnSelectItems(IList<object> items)
        {
            StatusReady();
            if (IsMultipleSelection)
            {
                SelectedItems.AddRange(items.Cast<TModel>());
                StatusMessage($"{SelectedItems.Count} items selected");
            }
        }

        public ICommand DeselectItemsCommand => new RelayCommand<IList<object>>(OnDeselectItems);
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

        public ICommand SelectRangesCommand => new RelayCommand<IndexRange[]>(OnSelectRanges);
        protected virtual void OnSelectRanges(IndexRange[] indexRanges)
        {
            SelectedIndexRanges = indexRanges;
            var count = SelectedIndexRanges?.Sum(r => r.Length) ?? 0;
            StatusMessage($"{count} items selected");
        }

        public ICommand DeleteSelectionCommand => new RelayCommand(OnDeleteSelection);

        protected abstract void OnNew();
        protected abstract void OnRefresh();
        protected abstract void OnDeleteSelection();

    }
}
