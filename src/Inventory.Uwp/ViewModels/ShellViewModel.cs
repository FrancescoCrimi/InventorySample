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
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Helpers;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Customers;
using Inventory.Uwp.ViewModels.Logs;
using Inventory.Uwp.ViewModels.Orders;
using Inventory.Uwp.ViewModels.Products;
using Inventory.Uwp.Views;
using Inventory.Uwp.Views.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Inventory.Uwp.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator;
        private readonly KeyboardAccelerator _backKeyboardAccelerator;
        private readonly ILogger<ShellViewModel> logger;
        private readonly NavigationService navigationService;
        private readonly LogService logService;
        private IList<KeyboardAccelerator> _keyboardAccelerators;
        private readonly CoreDispatcher dispatcher;

        public ShellViewModel(ILogger<ShellViewModel> logger,
                              NavigationService navigationService,
                              LogService logService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            this.logService = logService;
            _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
            _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
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


        private string _message = "Ready";
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }


        private int logNewCount = 10;
        public int LogNewCount
        {
            get => logNewCount;
            set => SetProperty(ref logNewCount, value);
        }


        private bool _isBackEnabled;
        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { SetProperty(ref _isBackEnabled, value); }
        }


        private ICommand _loadedCommand;
        public ICommand LoadedCommand => _loadedCommand ??
            (_loadedCommand = new RelayCommand(OnLoaded));
        private async void OnLoaded()
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            _keyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            _keyboardAccelerators.Add(_backKeyboardAccelerator);

            await UpdateAppLogBadge();
            LogService.AddLogEvent += Logging_AddLogEvent;
            Messenger.Register<StatusMessage>(this, OnStatusMessage);
        }


        private RelayCommand unloadedCommand;
        public ICommand UnloadedCommand => unloadedCommand ??
                    (unloadedCommand = new RelayCommand(Unloaded));
        private void Unloaded()
        {
            //MessageService.Unsubscribe(this);
            LogService.AddLogEvent -= Logging_AddLogEvent;
            Messenger.UnregisterAll(this);
        }


        private ICommand _itemInvokedCommand;
        public ICommand ItemInvokedCommand => _itemInvokedCommand ??
            (_itemInvokedCommand = new RelayCommand<WinUI.NavigationViewItemInvokedEventArgs>(OnItemInvoked));
        private void OnItemInvoked(WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                navigationService.Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
            }
            else
            {
                var selectedItem = args.InvokedItemContainer as WinUI.NavigationViewItem;
                var pageType = selectedItem?.GetValue(NavHelper.NavigateToProperty) as Type;

                if (pageType != null)
                {
                    switch (pageType.Name)
                    {
                        case "DashboardPage":
                            navigationService.Navigate(pageType);
                            break;
                        case "CustomersPage":
                            navigationService.Navigate(pageType, new CustomerListArgs());
                            break;
                        case "OrdersPage":
                            navigationService.Navigate(pageType, new OrderListArgs());
                            break;
                        case "ProductsPage":
                            navigationService.Navigate(pageType, new ProductListArgs());
                            break;
                        case "LogsPage":
                            navigationService.Navigate(pageType, new LogListArgs());
                            //await logService.MarkAllAsReadAsync();
                            //await UpdateAppLogBadge();
                            break;
                        //case "SettingsViewModel":
                        //    navigationService.Navigate(viewModel, new SettingsArgs());
                        //    break;
                        default:
                            navigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
                            break;
                    }
                }
            }
        }


        private RelayCommand backRequestedCommand;
        public ICommand BackRequestedCommand => backRequestedCommand ??
            (backRequestedCommand = new RelayCommand(OnBackRequested));
        private void OnBackRequested()
        {
            navigationService.GoBack();
        }


        public void Initialize(Frame frame, IList<KeyboardAccelerator> keyboardAccelerators)
        {
            _keyboardAccelerators = keyboardAccelerators;
            navigationService.Frame = frame;
            navigationService.NavigationFailed += Frame_NavigationFailed;
            navigationService.Navigated += Frame_Navigated;
            navigationService.OnCurrentPageCanGoBackChanged += OnCurrentPageCanGoBackChanged;
        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void OnCurrentPageCanGoBackChanged(object sender, bool currentPageCanGoBack)
            => IsBackEnabled = navigationService.CanGoBack || currentPageCanGoBack;

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = navigationService.CanGoBack;
        }

        private KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = navigationService.GoBack();
            args.Handled = result;
        }

        private void SetStatus(string message)
        {
            message = message ?? "";
            message = message.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
            Message = message;
        }

        private void OnStatusMessage(object recipient, StatusMessage message)
        {
            //    throw new NotImplementedException();
            //}

            //private async void OnMessage(ViewModelBase viewModel, string message, string status)
            //{
            switch (message.Value)
            {
                case "StatusMessage":
                case "StatusError":
                    IsError = message.Value == "StatusError";
                    SetStatus(message.Args);
                    break;

                case "EnableThisView":
                case "DisableThisView":
                    IsEnabled = message.Value == "EnableThisView";
                    SetStatus(message.Args);
                    break;

                case "EnableOtherViews":
                case "DisableOtherViews":
                    IsEnabled = message.Value == "EnableOtherViews";
                    SetStatus(message.Args);
                    break;

                case "EnableAllViews":
                case "DisableAllViews":
                    IsEnabled = message.Value == "EnableAllViews";
                    SetStatus(message.Args);
                    break;
            }
        }

        private async Task UpdateAppLogBadge()
        {
            //var result = await logService.GetLogsCountAsync(new DataRequest<Log> { Where = r => !r.IsRead });
            ////LogNewCount = await logService.GetLogsCountAsync(new DataRequest<Log> { Where = r => !r.IsRead });
            //////AppLogsItem.Badge = count > 0 ? count.ToString() : null;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var result = await logService.GetLogsCountAsync(new DataRequest<Log> { Where = r => !r.IsRead });
                LogNewCount = result;
            });
        }

        private async void Logging_AddLogEvent(object sender, EventArgs e)
        {
            await Task.CompletedTask;
            //await UpdateAppLogBadge();
        }
    }
}
