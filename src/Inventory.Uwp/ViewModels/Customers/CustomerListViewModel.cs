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
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Interface.Dto;
using Inventory.Interface;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.Services.VirtualCollections;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.Views.Customers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.Customers
{
    public class CustomerListViewModel : GenericListViewModel<CustomerDto>
    {
        private readonly ILogger _logger;
        private readonly ICustomerServiceFacade _customerService;
        private readonly NavigationService _navigationService;
        private readonly WindowManagerService _windowService;
        private readonly CustomerCollection _collection;

        public CustomerListViewModel(ILogger<CustomerListViewModel> logger,
                                     ICustomerServiceFacade customerService,
                                     NavigationService navigationService,
                                     WindowManagerService windowService,
                                     CustomerCollection collection)
            : base()
        {
            _logger = logger;
            _customerService = customerService;
            _navigationService = navigationService;
            _windowService = windowService;
            _collection = collection;
            Items = _collection;
        }

        public CustomerListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(CustomerListArgs args)
        {
            ViewModelArgs = args ?? new CustomerListArgs();
            Query = ViewModelArgs.Query;

            StartStatusMessage("Loading customers...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Customers loaded");
            }
        }

        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<CustomerListViewModel>(this, OnMessage);
            //MessageService.Subscribe<CustomerDetailsViewModel>(this, OnMessage);
            Messenger.Register<ViewModelsMessage<CustomerDto>>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
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
            ItemsCount = 0;

            //todo: questa try é forse inutile, verificare virtualcollection loadasync
            try
            {
                DataRequest<Customer> request = BuildDataRequest();
                await _collection.LoadAsync(request);
            }
            catch (Exception ex)
            {
                Items = new List<CustomerDto>();
                StatusError($"Error loading Customers: {ex.Message}");
                _logger.LogError(LogEvents.Refresh, ex, "Error loading Customers");
                isOk = false;
            }

            ItemsCount = Items.Count;
            OnPropertyChanged(nameof(Title));
            return isOk;
        }

        public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
        private async void OnOpenInNewView()
        {
            if (SelectedItem != null)
            {
                await _windowService.OpenWindow(typeof(CustomerView), new CustomerDetailsArgs { CustomerId = SelectedItem.Id });
            }
        }

        protected override async void OnNew()
        {
            if (IsMainView)
            {
                await _windowService.OpenWindow(typeof(CustomerView), new CustomerDetailsArgs());
            }
            else
            {
                _navigationService.Navigate(typeof(CustomerView), new CustomerDetailsArgs());
            }

            StatusReady();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Loading customers...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Customers loaded");
            }
        }

        protected override async void OnDeleteSelection()
        {
            StatusReady();
            if (await _windowService.OpenDialog("Confirm Delete", "Are you sure you want to delete selected customers?", "Ok", "Cancel"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} customers...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        //MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                        Messenger.Send(new ViewModelsMessage<CustomerDto>("ItemRangesDeleted", SelectedIndexRanges));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} customers...");
                        await DeleteItemsAsync(SelectedItems);
                        //MessageService.Send(this, "ItemsDeleted", SelectedItems);
                        Messenger.Send(new ViewModelsMessage<CustomerDto>("ItemsDeleted", SelectedItems));
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Customers: {ex.Message}");
                    _logger.LogError(LogEvents.Delete, ex, "Error deleting {count} Customers");
                    count = 0;
                }
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    EndStatusMessage($"{count} customers deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<CustomerDto> models)
        {
            foreach (var model in models)
            {
                await _customerService.DeleteCustomerAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Customer> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await _customerService.DeleteCustomerRangeAsync(range.Index, range.Length, request);
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

        private async void OnMessage(object recipient, ViewModelsMessage<CustomerDto> message)
        {
            switch (message.Value)
            {
                case "NewItemSaved":
                case "ItemDeleted":
                case "ItemsDeleted":
                case "ItemRangesDeleted":
                    await RefreshAsync();
                    break;
            }
        }
    }
}
