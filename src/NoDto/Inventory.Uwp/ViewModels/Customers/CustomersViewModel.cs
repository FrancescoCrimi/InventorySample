// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Domain.Aggregates.CustomerAggregate;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.ViewModels.Orders;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.Customers
{
    public class CustomersViewModel : ViewModelBase
    {
        private readonly ILogger _logger;

        public CustomersViewModel(ILogger<CustomersViewModel> logger,
                                  CustomerListViewModel customerListViewModel,
                                  CustomerDetailsViewModel customerDetailsViewModel,
                                  OrderListViewModel orderListViewModel)
            : base()
        {
            _logger = logger;
            CustomerList = customerListViewModel;
            CustomerDetails = customerDetailsViewModel;
            CustomerOrders = orderListViewModel;
        }

        public CustomerListViewModel CustomerList { get; }

        public CustomerDetailsViewModel CustomerDetails { get; }

        public OrderListViewModel CustomerOrders { get; }

        public async Task LoadAsync(CustomerListArgs args)
        {
            await CustomerList.LoadAsync(args);
            if (args != null)
            {
                IsMainView = args.IsMainView;
                CustomerList.IsMainView = args.IsMainView;
                CustomerDetails.IsMainView = args.IsMainView;
            }
        }

        public void Unload()
        {
            CustomerDetails.CancelEdit();
            CustomerList.Unload();
        }

        public void Subscribe()
        {
            Messenger.Register<ViewModelsMessage<Customer>>(this, OnMessage);
            CustomerList.Subscribe();
            CustomerDetails.Subscribe();
            CustomerOrders.Subscribe();
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
            CustomerList.Unsubscribe();
            CustomerDetails.Unsubscribe();
            CustomerOrders.Unsubscribe();
        }

        private async void OnMessage(object recipient, ViewModelsMessage<Customer> message)
        {
            if (message.Value == "ItemSelected")
            {
                if (message.Id != 0)
                {
                    //TODO: rendere il metodo OnItemSelected cancellabile
                    await OnItemSelected(message.Id);
                }
            }
        }

        private async Task OnItemSelected(long id)
        {
            if (CustomerDetails.IsEditMode)
            {
                StatusReady();
                CustomerDetails.CancelEdit();
            }
            CustomerOrders.IsMultipleSelection = false;
            if (!CustomerList.IsMultipleSelection)
            {
                if (id != 0)
                {
                    await PopulateDetails(id);
                    await PopulateOrders(id);
                }
            }
        }

        private async Task PopulateDetails(long id)
        {
            try
            {
                await CustomerDetails.LoadAsync(new CustomerDetailsArgs { CustomerId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadDetails, ex, "Load Details");
            }
        }

        private async Task PopulateOrders(long id)
        {
            try
            {
                await CustomerOrders.LoadAsync(new OrderListArgs { CustomerId = id }, silent: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadOrders, ex, "Load Orders");
            }
        }
    }
}
