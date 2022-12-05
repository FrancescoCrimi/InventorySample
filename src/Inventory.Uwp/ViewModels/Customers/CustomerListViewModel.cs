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
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Services;
using Inventory.Uwp.Services.VirtualCollections;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.Library.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.Customers
{
    #region CustomerListArgs
    public class CustomerListArgs
    {
        public static CustomerListArgs CreateEmpty() => new CustomerListArgs { IsEmpty = true };

        public CustomerListArgs()
        {
            OrderBy = r => r.FirstName;
        }

        public bool IsEmpty { get; set; }

        public string Query { get; set; }

        //public bool IsMainView { get; set; }

        public Expression<Func<Customer, object>> OrderBy { get; set; }

        public Expression<Func<Customer, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class CustomerListViewModel : GenericListViewModel<CustomerDto>
    {
        private readonly ILogger<CustomerListViewModel> logger;
        private readonly CustomerServiceFacade customerService;
        private readonly NavigationService navigationService;
        private readonly WindowService windowService;
        private readonly CustomerCollection collection;

        public CustomerListViewModel(ILogger<CustomerListViewModel> logger,
                                     CustomerServiceFacade customerService,
                                     NavigationService navigationService,
                                     WindowService windowService)
            : base()
        {
            this.logger = logger;
            this.customerService = customerService;
            this.navigationService = navigationService;
            this.windowService = windowService;
            collection = new CustomerCollection(customerService);
            Items = collection;
        }

        public CustomerListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(CustomerListArgs args)
        {
            ViewModelArgs = args ?? CustomerListArgs.CreateEmpty();
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
            Messenger.Register<ItemMessage<IList<CustomerDto>>>(this, OnCustomerListMessage);
            Messenger.Register<ItemMessage<IList<IndexRange>>>(this, OnIndexRangeListMessage);

            //MessageService.Subscribe<CustomerDetailsViewModel>(this, OnMessage);
            Messenger.Register<ItemMessage<CustomerDto>>(this, OnCustomerMessage);
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
            //Items = null;
            ItemsCount = 0;
            //SelectedItem = null;

            try
            {
                if (!ViewModelArgs.IsEmpty)
                {
                    DataRequest<Customer> request = BuildDataRequest();
                    await collection.LoadAsync(request);
                }
            }
            catch (Exception ex)
            {
                Items = new List<CustomerDto>();
                StatusError($"Error loading Customers: {ex.Message}");
                logger.LogError(ex, "Refresh");
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
                await windowService.OpenInNewWindow<CustomerDetailsViewModel>(new CustomerDetailsArgs { CustomerID = SelectedItem.CustomerID });
            }
        }

        protected override async void OnNew()
        {
            if (IsMainView)
            {
                await windowService.OpenInNewWindow<CustomerDetailsViewModel>(new CustomerDetailsArgs());
            }
            else
            {
                //navigationService.Navigate<CustomerDetailsViewModel>(new CustomerDetailsArgs());
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
            if (await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete selected customers?", "Ok", "Cancel"))
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
                        Messenger.Send(new ItemMessage<IList<IndexRange>>(SelectedIndexRanges, "ItemRangesDeleted"));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} customers...");
                        await DeleteItemsAsync(SelectedItems);
                        //MessageService.Send(this, "ItemsDeleted", SelectedItems);
                        Messenger.Send(new ItemMessage<IList<CustomerDto>>(SelectedItems, "ItemsDeleted"));
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Customers: {ex.Message}");
                    logger.LogError(ex, "Delete");
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

        //private async void OnMessage(ViewModelBase sender, string message, object args)
        //{
        //    switch (message)
        //    {
        //        case "NewItemSaved":
        //        case "ItemDeleted":
        //        case "ItemsDeleted":
        //        case "ItemRangesDeleted":
        //            //await ContextService.RunAsync(async () =>
        //            //{
        //            await RefreshAsync();
        //            //});
        //            break;
        //    }
        //}

        private async void OnIndexRangeListMessage(object recipient, ItemMessage<IList<IndexRange>> message)
        {
            switch (message.Message)
            {
                //case "NewItemSaved":
                //case "ItemDeleted":
                //case "ItemsDeleted":
                case "ItemRangesDeleted":
                    await RefreshAsync();
                    break;
            }
        }

        private async void OnCustomerListMessage(object recipient, ItemMessage<IList<CustomerDto>> message)
        {
            switch (message.Message)
            {
                //case "NewItemSaved":
                //case "ItemDeleted":
                case "ItemsDeleted":
                    //case "ItemRangesDeleted":
                    await RefreshAsync();
                    break;
            }
        }

        private async void OnCustomerMessage(object recipient, ItemMessage<CustomerDto> message)
        {
            switch (message.Message)
            {
                case "NewItemSaved":
                case "ItemDeleted":
                    //case "ItemsDeleted":
                    //case "ItemRangesDeleted":
                    await RefreshAsync();
                    break;
            }
        }
    }
}
