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
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Inventory.Uwp.ViewModels;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Orders;

namespace Inventory.Uwp.ViewModels.Customers
{
    public class CustomersViewModel : ViewModelBase
    {
        private readonly ILogger<CustomersViewModel> logger;
        private readonly CustomerServiceFacade customerService;

        public CustomersViewModel(ILogger<CustomersViewModel> logger,
                                  CustomerServiceFacade customerService,
                                  CustomerListViewModel customerListViewModel,
                                  CustomerDetailsViewModel customerDetailsViewModel,
                                  OrderListViewModel orderListViewModel)
            : base()
        {
            this.logger = logger;
            this.customerService = customerService;
            CustomerList = customerListViewModel;
            CustomerDetails = customerDetailsViewModel;
            CustomerOrders = orderListViewModel;
        }

        public CustomerListViewModel CustomerList { get; set; }
        public CustomerDetailsViewModel CustomerDetails { get; set; }
        public OrderListViewModel CustomerOrders { get; set; }

        public async Task LoadAsync(CustomerListArgs args)
        {
            await CustomerList.LoadAsync(args);
            //IsMainView = args.IsMainView;
            //OnPropertyChanged(nameof(IsMainView));
        }

        public void Unload()
        {
            CustomerDetails.CancelEdit();
            CustomerList.Unload();
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<CustomerListViewModel>(this, OnMessage);
            Messenger.Register<ItemMessage<CustomerDto>>(this, OnCustomerMessage);
            CustomerList.Subscribe();
            CustomerDetails.Subscribe();
            CustomerOrders.Subscribe();
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.Unregister<ItemMessage<CustomerDto>>(this);
            CustomerList.Unsubscribe();
            CustomerDetails.Unsubscribe();
            CustomerOrders.Unsubscribe();
        }

        private async void OnCustomerMessage(object recipient, ItemMessage<CustomerDto> message)
        {
            if (message.Message == "ItemSelected")
            {
                if (message.Value.CustomerID != 0)
                {
                    //TODO: rendere il metodo OnItemSelected cancellabile
                    await OnItemSelected();
                }
            }
        }

        //private async void OnMessage(CustomerListViewModel viewModel, string message, object args)
        //{
        //    if (viewModel == CustomerList && message == "ItemSelected")
        //    {
        //        //await ContextService.RunAsync(() =>
        //        //{
        //           //await OnItemSelected();
        //        //});
        //    }
        //} 

        private async Task OnItemSelected()
        {
            if (CustomerDetails.IsEditMode)
            {
                StatusReady();
                CustomerDetails.CancelEdit();
            }
            CustomerOrders.IsMultipleSelection = false;
            var selected = CustomerList.SelectedItem;
            if (!CustomerList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                    await PopulateOrders(selected);
                }
            }
            CustomerDetails.Item = selected;
        }

        private async Task PopulateDetails(CustomerDto selected)
        {
            try
            {
                var model = await customerService.GetCustomerAsync(selected.CustomerID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Details");
            }
        }

        private async Task PopulateOrders(CustomerDto selectedItem)
        {
            try
            {
                if (selectedItem != null)
                {
                    await CustomerOrders.LoadAsync(new OrderListArgs { CustomerID = selectedItem.CustomerID }, silent: true);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Orders");
            }
        }
    }
}
