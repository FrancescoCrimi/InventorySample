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
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Helpers;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.Views.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Inventory.Uwp.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly ILogger<ShellViewModel> logger;
        private readonly NavigationService navigationService;
        private readonly LogService logService;
        private readonly CoreDispatcher dispatcher;
        private bool _isEnabled = true;
        private bool _isError = false;
        private string _message = "Ready";
        private int logCount = 10;
        private bool _isBackEnabled;
        private AsyncRelayCommand _loadedCommand;
        private RelayCommand unloadedCommand;
        private AsyncRelayCommand<WinUI.NavigationViewItemInvokedEventArgs> _itemInvokedCommand;
        private RelayCommand backRequestedCommand;

        public ShellViewModel(ILogger<ShellViewModel> logger,
                              NavigationService navigationService,
                              LogService logService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            this.logService = logService;
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public bool IsError
        {
            get => _isError;
            set => SetProperty(ref _isError, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public int LogCount
        {
            get => logCount;
            set => SetProperty(ref logCount, value);
        }

        public bool IsBackEnabled
        {
            get => _isBackEnabled;
            set => SetProperty(ref _isBackEnabled, value);
        }

        public IAsyncRelayCommand LoadedCommand => _loadedCommand
            ?? (_loadedCommand = new AsyncRelayCommand(async () =>
            {
                await UpdateAppLogBadge();
                LogService.AddLogEvent += Logging_AddLogEvent;
                Messenger.Register<StatusMessage>(this, OnStatusMessage);
            }));

        public ICommand UnloadedCommand => unloadedCommand
            ?? (unloadedCommand = new RelayCommand(() =>
            {
                //MessageService.Unsubscribe(this);
                LogService.AddLogEvent -= Logging_AddLogEvent;
                Messenger.UnregisterAll(this);
            }));

        public IAsyncRelayCommand ItemInvokedCommand => _itemInvokedCommand
            ?? (_itemInvokedCommand = new AsyncRelayCommand<WinUI.NavigationViewItemInvokedEventArgs>(async (args) =>
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
                        navigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
                    }
            }
                await UpdateAppLogBadge();
            }));

        public ICommand BackRequestedCommand => backRequestedCommand
            ?? (backRequestedCommand = new RelayCommand(() => navigationService.GoBack()));


        public void Initialize(Frame frame)
        {
            navigationService.Frame = frame;
            navigationService.NavigationFailed += Frame_NavigationFailed;
            navigationService.Navigated += Frame_Navigated;
            //navigationService.OnCurrentPageCanGoBackChanged += OnCurrentPageCanGoBackChanged;
        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        //private void OnCurrentPageCanGoBackChanged(object sender, bool currentPageCanGoBack)
        //    => IsBackEnabled = navigationService.CanGoBack || currentPageCanGoBack;

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = navigationService.CanGoBack;
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
                LogCount = result;
            });
        }

        private async void Logging_AddLogEvent(object sender, EventArgs e)
        {
            await Task.CompletedTask;
            //await UpdateAppLogBadge();
        }
    }
}
