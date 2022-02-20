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
using Inventory.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Inventory.ViewModels
{
    public class MainShellViewModel : ObservableRecipient //ShellViewModel
    {
        private readonly NavigationItem DashboardItem = new NavigationItem(0xE80F, "Dashboard", typeof(DashboardViewModel));
        private readonly NavigationItem CustomersItem = new NavigationItem(0xE716, "Customers", typeof(CustomersViewModel));
        private readonly NavigationItem OrdersItem = new NavigationItem(0xE8A1, "Orders", typeof(OrdersViewModel));
        private readonly NavigationItem ProductsItem = new NavigationItem(0xE781, "Products", typeof(ProductsViewModel));
        private readonly NavigationItem AppLogsItem = new NavigationItem(0xE7BA, "Activity Log", typeof(AppLogsViewModel));
        private readonly NavigationItem SettingsItem = new NavigationItem(0x0000, "Settings", typeof(SettingsViewModel));
        private readonly ILogger<MainShellViewModel> logger;
        private readonly IMessageService messageService;
        private readonly IContextService contextService;
        private readonly INavigationService navigationService;
        private readonly IDialogService dialogService;

        public MainShellViewModel(ILogger<MainShellViewModel> logger,
                                  IMessageService messageService,
                                     IContextService contextService,
                                     INavigationService navigationService,
                                     IDialogService dialogService
            //ILoginService loginService,
            //ICommonServices commonServices
            )
        {
            this.logger = logger;
            this.messageService = messageService;
            this.contextService = contextService;
            this.navigationService = navigationService;
            this.dialogService = dialogService;
        }

        private object _selectedItem;
        public object SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private bool _isPaneOpen = true;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => SetProperty(ref _isPaneOpen, value);
        }

        private IEnumerable<NavigationItem> _items;
        public IEnumerable<NavigationItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public ShellArgs ViewModelArgs { get; protected set; }
        public UserInfo UserInfo { get; protected set; }
        public async Task LoadAsync(ShellArgs args)
        {
            Items =  GetItems().ToArray();
            await UpdateAppLogBadge();
            ViewModelArgs = args;
            if (ViewModelArgs != null)
            {
                UserInfo = ViewModelArgs.UserInfo;
                navigationService.Navigate(ViewModelArgs.ViewModel, ViewModelArgs.Parameter);
            }
             await Task.CompletedTask;
        }

        public void Subscribe()
        {
            messageService.Subscribe<ILogService, AppLog>(this, OnLogServiceMessage);

            //base.Subscribe();
            messageService.Subscribe<ViewModelBase, string>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
        }

        private async void OnMessage(ViewModelBase viewModel, string message, string status)
        {
            switch (message)
            {
                case "StatusMessage":
                case "StatusError":
                    if (viewModel.ContextService.ContextID == contextService.ContextID)
                    {
                        IsError = message == "StatusError";
                        SetStatus(status);
                    }
                    break;

                case "EnableThisView":
                case "DisableThisView":
                    if (viewModel.ContextService.ContextID == contextService.ContextID)
                    {
                        IsEnabled = message == "EnableThisView";
                        SetStatus(status);
                    }
                    break;

                case "EnableOtherViews":
                case "DisableOtherViews":
                    if (viewModel.ContextService.ContextID != contextService.ContextID)
                    {
                        await contextService.RunAsync(() =>
                        {
                            IsEnabled = message == "EnableOtherViews";
                            SetStatus(status);
                        });
                    }
                    break;

                case "EnableAllViews":
                case "DisableAllViews":
                    await contextService.RunAsync(() =>
                    {
                        IsEnabled = message == "EnableAllViews";
                        SetStatus(status);
                    });
                    break;
            }
        }

        private void SetStatus(string message)
        {
            message = message ?? "";
            message = message.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
            Message = message;
        }

        private string _message = "Ready";
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        private bool _isError = false;
        public bool IsError
        {
            get => _isError;
            set => SetProperty(ref _isError, value);
        }

        public void Unload()
        {
            //base.Unload();
        }

        public async void NavigateTo(Type viewModel)
        {
            switch (viewModel.Name)
            {
                case "DashboardViewModel":
                    navigationService.Navigate(viewModel);
                    break;
                case "CustomersViewModel":
                    navigationService.Navigate(viewModel, new CustomerListArgs());
                    break;
                case "OrdersViewModel":
                    navigationService.Navigate(viewModel, new OrderListArgs());
                    break;
                case "ProductsViewModel":
                    navigationService.Navigate(viewModel, new ProductListArgs());
                    break;
                case "AppLogsViewModel":
                    navigationService.Navigate(viewModel, new AppLogListArgs());

                    //TODO: LogService
                    //await LogService.MarkAllAsReadAsync();

                    await UpdateAppLogBadge();
                    break;
                case "SettingsViewModel":
                    navigationService.Navigate(viewModel, new SettingsArgs());
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private IEnumerable<NavigationItem> GetItems()
        {
            yield return DashboardItem;
            yield return CustomersItem;
            yield return OrdersItem;
            yield return ProductsItem;
            yield return AppLogsItem;
        }

        private async void OnLogServiceMessage(ILogService logService, string message, AppLog log)
        {
            if (message == "LogAdded")
            {
                await contextService.RunAsync(async () =>
                {
                    await UpdateAppLogBadge();
                });
            }
        }

        private async Task UpdateAppLogBadge()
        {
            //TODO: LogService
            //int count = await LogService.GetLogsCountAsync(new DataRequest<AppLog> { Where = r => !r.IsRead });
            //AppLogsItem.Badge = count > 0 ? count.ToString() : null;
            await Task.CompletedTask;
        }
    }
}
