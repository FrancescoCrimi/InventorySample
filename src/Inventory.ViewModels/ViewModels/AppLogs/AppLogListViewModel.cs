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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Inventory.Data;
using Inventory.Models;
using Inventory.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

namespace Inventory.ViewModels
{
    public class AppLogListViewModel : ObservableRecipient //GenericListViewModel<AppLogModel>
    {
        private readonly ILogger<AppLogListViewModel> logger;
        private readonly IMessageService messageService;
        private readonly IContextService contextService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;

        public AppLogListViewModel(ILogger<AppLogListViewModel> logger,
                                   IMessageService messageService,
                                   IContextService contextService,
                                   IDialogService dialogService,
                                   INavigationService navigationService)
        {
            this.logger = logger;
            this.messageService = messageService;
            this.contextService = contextService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
        }





        public ICommand RefreshCommand => new RelayCommand(OnRefresh);

        public ICommand StartSelectionCommand => new RelayCommand(OnStartSelection);
        virtual protected void OnStartSelection()
        {
            //StatusMessage("Start selection");
            SelectedItem = null;
            SelectedItems = new List<AppLogModel>();
            SelectedIndexRanges = null;
            IsMultipleSelection = true;
        }

        public ICommand CancelSelectionCommand => new RelayCommand(OnCancelSelection);
        virtual protected void OnCancelSelection()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");

            SelectedItems = null;
            SelectedIndexRanges = null;
            IsMultipleSelection = false;
            SelectedItem = Items?.FirstOrDefault();
        }

        public ICommand DeselectItemsCommand => new RelayCommand<IList<object>>(OnDeselectItems);
        virtual protected void OnDeselectItems(IList<object> items)
        {
            if (items?.Count > 0)
            {
                //StatusReady();
                messageService.Send(this, "StatusMessage", "Ready");
            }
            if (IsMultipleSelection)
            {
                foreach (AppLogModel item in items)
                {
                    SelectedItems.Remove(item);
                }
                //StatusMessage($"{SelectedItems.Count} items selected");
            }
        }

        public ICommand SelectRangesCommand => new RelayCommand<IndexRange[]>(OnSelectRanges);
        virtual protected void OnSelectRanges(IndexRange[] indexRanges)
        {
            SelectedIndexRanges = indexRanges;
            int count = SelectedIndexRanges?.Sum(r => r.Length) ?? 0;
            //StatusMessage($"{count} items selected");
        }

        public ICommand SelectItemsCommand => new RelayCommand<IList<object>>(OnSelectItems);
        virtual protected void OnSelectItems(IList<object> items)
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            if (IsMultipleSelection)
            {
                SelectedItems.AddRange(items.Cast<AppLogModel>());
                //StatusMessage($"{SelectedItems.Count} items selected");
            }
        }

        public ICommand DeleteSelectionCommand => new RelayCommand(OnDeleteSelection);







        public string Title => String.IsNullOrEmpty(Query) ? $" ({ItemsCount})" : $" ({ItemsCount} for \"{Query}\")";

        public bool IsMainView => contextService.IsMainView;

        public AppLogListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(AppLogListArgs args)
        {
            ViewModelArgs = args ?? AppLogListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            //StartStatusMessage("Loading logs...");
            if (await RefreshAsync())
            {
                //EndStatusMessage("Logs loaded");
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            messageService.Subscribe<AppLogListViewModel>(this, OnMessage);
            messageService.Subscribe<AppLogDetailsViewModel>(this, OnMessage);
            messageService.Subscribe<ILogService, AppLog>(this, OnLogServiceMessage);
        }
        public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
        }

        public AppLogListArgs CreateArgs()
        {
            return new AppLogListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
        }

        public async Task<bool> RefreshAsync()
        {
            bool isOk = true;

            Items = null;
            ItemsCount = 0;
            SelectedItem = null;

            //try
            //{
            Items = await GetItemsAsync();
            //}
            //catch (Exception ex)
            //{
            //    Items = new List<AppLogModel>();
            //    StatusError($"Error loading Logs: {ex.Message}");
            //    LogException("AppLogs", "Refresh", ex);
            //    isOk = false;
            //}

            ItemsCount = Items.Count;
            if (!IsMultipleSelection)
            {
                SelectedItem = Items.FirstOrDefault();
            }
            OnPropertyChanged(nameof(Title));

            return isOk;
        }

        private  Task<IList<AppLogModel>> GetItemsAsync()
        {
            //TODO: LogService
            //if (!ViewModelArgs.IsEmpty)
            //{
            //    DataRequest<AppLog> request = BuildDataRequest();
            //    return await LogService.GetLogsAsync(request);
            //}
            return Task.Run(() => (IList<AppLogModel>)new List<AppLogModel>());
            //return new List<AppLogModel>();
        }

        protected void OnNew()
        {
            throw new NotImplementedException();
        }

        protected async void OnRefresh()
        {
            //StartStatusMessage("Loading logs...");
            if (await RefreshAsync())
            {
                //EndStatusMessage("Logs loaded");
            }
        }

        protected async void OnDeleteSelection()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            if (await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected logs?", "Ok", "Cancel"))
            {
                int count = 0;
                //try
                //{
                if (SelectedIndexRanges != null)
                {
                    count = SelectedIndexRanges.Sum(r => r.Length);
                    //StartStatusMessage($"Deleting {count} logs...");
                    await DeleteRangesAsync(SelectedIndexRanges);
                    messageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                }
                else if (SelectedItems != null)
                {
                    count = SelectedItems.Count();
                    //StartStatusMessage($"Deleting {count} logs...");
                    await DeleteItemsAsync(SelectedItems);
                    messageService.Send(this, "ItemsDeleted", SelectedItems);
                }
                //}
                //catch (Exception ex)
                //{
                //    StatusError($"Error deleting {count} Logs: {ex.Message}");
                //    LogException("AppLogs", "Delete", ex);
                //    count = 0;
                //}
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    //EndStatusMessage($"{count} logs deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<AppLogModel> models)
        {
            foreach (var model in models)
            {
                //TODO: LogService
                //await LogService.DeleteLogAsync(model);
                await Task.CompletedTask;
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<AppLog> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                //TODO: LogService
                //await LogService.DeleteLogRangeAsync(range.Index, range.Length, request);
                await Task.CompletedTask;
            }
        }

        private DataRequest<AppLog> BuildDataRequest()
        {
            return new DataRequest<AppLog>()
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
        }

        private async void OnMessage(ObservableRecipient sender, string message, object args)
        {
            switch (message)
            {
                case "NewItemSaved":
                case "ItemDeleted":
                case "ItemsDeleted":
                case "ItemRangesDeleted":
                    await contextService.RunAsync(async () =>
                    {
                        await RefreshAsync();
                    });
                    break;
            }
        }

        private async void OnLogServiceMessage(ILogService logService, string message, AppLog log)
        {
            if (message == "LogAdded")
            {
                await contextService.RunAsync(async () =>
                {
                    await RefreshAsync();
                });
            }
        }







        private string _query = null;
        public string Query
        {
            get => _query;
            set => SetProperty(ref _query, value);
        }

        private IList<AppLogModel> _items = null;
        public IList<AppLogModel> Items
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

        private AppLogModel _selectedItem = default(AppLogModel);
        public AppLogModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    if (!IsMultipleSelection)
                    {
                        messageService.Send(this, "ItemSelected", _selectedItem);
                    }
                }
            }
        }

        private bool _isMultipleSelection = false;
        public bool IsMultipleSelection
        {
            get => _isMultipleSelection;
            set => SetProperty(ref _isMultipleSelection, value);
        }

        public IndexRange[] SelectedIndexRanges { get; protected set; }

        public List<AppLogModel> SelectedItems { get; protected set; }
    }
}
