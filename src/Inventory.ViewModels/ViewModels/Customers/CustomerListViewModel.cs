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

using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Inventory.Data;
using Inventory.Models;
using Inventory.Services;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;

namespace Inventory.ViewModels
{
    public class CustomerListViewModel : ObservableRecipient //GenericListViewModel<CustomerModel>
    {
        private readonly ILogger<CustomerListViewModel> logger;
        private readonly IMessageService messageService;
        private readonly IContextService contextService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly ICustomerService customerService;

        public CustomerListViewModel(ILogger<CustomerListViewModel> logger,
                                     IMessageService messageService,
                                     IContextService contextService,
                                     IDialogService dialogService,
                                     INavigationService navigationService,
                                     ICustomerService customerService)
        {
            this.logger = logger;
            this.messageService = messageService;
            this.contextService = contextService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.customerService = customerService;
        }

        public ICommand NewCommand => new RelayCommand(OnNew);

        public ICommand RefreshCommand => new RelayCommand(OnRefresh);

        public ICommand StartSelectionCommand => new RelayCommand(OnStartSelection);
        virtual protected void OnStartSelection()
        {
            //StatusMessage("Start selection");
            SelectedItem = null;
            SelectedItems = new List<CustomerModel>();
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

        public ICommand SelectItemsCommand => new RelayCommand<IList<object>>(OnSelectItems);
        virtual protected void OnSelectItems(IList<object> items)
        {
            messageService.Send(this, "StatusMessage", "Ready");
            if (IsMultipleSelection)
            {
                SelectedItems.AddRange(items.Cast<CustomerModel>());
                //StatusMessage($"{SelectedItems.Count} items selected");
            }
        }

        public ICommand DeselectItemsCommand => new RelayCommand<IList<object>>(OnDeselectItems);
        virtual protected void OnDeselectItems(IList<object> items)
        {
            if (items?.Count > 0)
            {
                messageService.Send(this, "StatusMessage", "Ready");
            }
            if (IsMultipleSelection)
            {
                foreach (CustomerModel item in items)
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

        public ICommand DeleteSelectionCommand => new RelayCommand(OnDeleteSelection);





        public string Title => String.IsNullOrEmpty(Query) ? $" ({ItemsCount})" : $" ({ItemsCount} for \"{Query}\")";

        public bool IsMainView => contextService.IsMainView;

        public CustomerListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(CustomerListArgs args)
        {
            ViewModelArgs = args ?? CustomerListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            //StartStatusMessage("Loading customers...");
            if (await RefreshAsync())
            {
                //EndStatusMessage("Customers loaded");
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            messageService.Subscribe<CustomerListViewModel>(this, OnMessage);
            messageService.Subscribe<CustomerDetailsViewModel>(this, OnMessage);
        }
        public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
        }

        public CustomerListArgs CreateArgs()
        {
            return new CustomerListArgs
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
            //    Items = new List<CustomerModel>();
            //    StatusError($"Error loading Customers: {ex.Message}");
            //    LogException("Customers", "Refresh", ex);
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

        private async Task<IList<CustomerModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<Customer> request = BuildDataRequest();
                return await customerService.GetCustomersAsync(request);
            }
            return new List<CustomerModel>();
        }

        public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
        private async void OnOpenInNewView()
        {
            if (SelectedItem != null)
            {
                await navigationService.CreateNewViewAsync<CustomerDetailsViewModel>(new CustomerDetailsArgs { CustomerID = SelectedItem.CustomerID });
            }
        }

        protected  async void OnNew()
        {
            if (IsMainView)
            {
                await navigationService.CreateNewViewAsync<CustomerDetailsViewModel>(new CustomerDetailsArgs());
            }
            else
            {
                navigationService.Navigate<CustomerDetailsViewModel>(new CustomerDetailsArgs());
            }

            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
        }

        protected  async void OnRefresh()
        {
            //StartStatusMessage("Loading customers...");
            if (await RefreshAsync())
            {
                //EndStatusMessage("Customers loaded");
            }
        }

        protected  async void OnDeleteSelection()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            if (await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected customers?", "Ok", "Cancel"))
            {
                int count = 0;
                //try
                //{
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        //StartStatusMessage($"Deleting {count} customers...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                    messageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        //StartStatusMessage($"Deleting {count} customers...");
                        await DeleteItemsAsync(SelectedItems);
                    messageService.Send(this, "ItemsDeleted", SelectedItems);
                    }
                //}
                //catch (Exception ex)
                //{
                //    StatusError($"Error deleting {count} Customers: {ex.Message}");
                //    LogException("Customers", "Delete", ex);
                //    count = 0;
                //}
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    //EndStatusMessage($"{count} customers deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<CustomerModel> models)
        {
            foreach (var model in models)
            {
                await customerService.DeleteCustomerAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Customer> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await customerService.DeleteCustomerRangeAsync(range.Index, range.Length, request);
            }
        }

        private DataRequest<Customer> BuildDataRequest()
        {
            return new DataRequest<Customer>()
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







        private string _query = null;
        public string Query
        {
            get => _query;
            set => SetProperty(ref _query, value);
        }

        private IList<CustomerModel> _items = null;
        public IList<CustomerModel> Items
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

        private CustomerModel _selectedItem = default(CustomerModel);
        public CustomerModel SelectedItem
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

        public List<CustomerModel> SelectedItems { get; protected set; }
    }
}
