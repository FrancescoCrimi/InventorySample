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
using System.Threading.Tasks;

using CiccioSoft.Inventory.Models;
using CiccioSoft.Inventory.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace CiccioSoft.Inventory.ViewModels
{
    public class CustomersViewModel : ViewModelBase
    {
        private readonly ILogger<CustomersViewModel> logger;
        private readonly ICustomerService customerService;

        public CustomersViewModel(ILogger<CustomersViewModel> logger,
                                  ICustomerService customerService,
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
            Messenger.Register<ItemMessage<CustomerModel>>(this, OnCustomerMessage);
            CustomerList.Subscribe();
            CustomerDetails.Subscribe();
            CustomerOrders.Subscribe();
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.Unregister<ItemMessage<CustomerModel>>(this);
            CustomerList.Unsubscribe();
            CustomerDetails.Unsubscribe();
            CustomerOrders.Unsubscribe();
        }

        private async void OnCustomerMessage(object recipient, ItemMessage<CustomerModel> message)
        {
            if (/*recipient == CustomerList &&*/ message.Message == "ItemSelected")
            {
                await OnItemSelected();
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

        private async Task PopulateDetails(CustomerModel selected)
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

        private async Task PopulateOrders(CustomerModel selectedItem)
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
