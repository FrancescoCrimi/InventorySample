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

using System.Threading.Tasks;

using Inventory.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Inventory.ViewModels
{

    public class ShellViewModel : ObservableRecipient
    {
        private readonly ILogger<ShellViewModel> logger;
        private readonly IMessageService messageService;
        private readonly INavigationService navigationService;
        private readonly IContextService contextService;

        public ShellViewModel(ILogger<ShellViewModel> logger,
            IMessageService messageService,
                              INavigationService navigationService,
                              //ILoginService loginService,
                              IContextService contextService)
        {
            this.logger = logger;
            this.messageService = messageService;
            this.navigationService = navigationService;
            this.contextService = contextService;
            //IsLocked = !loginService.IsAuthenticated;
        }

        private bool _isLocked = false;
        public bool IsLocked
        {
            get => _isLocked;
            set => SetProperty(ref _isLocked, value);
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        private string _message = "Ready";
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private bool _isError = false;
        public bool IsError
        {
            get => _isError;
            set => SetProperty(ref _isError, value);
        }

        public UserInfo UserInfo { get; protected set; }

        public ShellArgs ViewModelArgs { get; protected set; }

        virtual public Task LoadAsync(ShellArgs args)
        {
            ViewModelArgs = args;
            if (ViewModelArgs != null)
            {
                UserInfo = ViewModelArgs.UserInfo;
                navigationService.Navigate(ViewModelArgs.ViewModel, ViewModelArgs.Parameter);
            }
            return Task.CompletedTask;
        }

        virtual public void Unload()
        {
        }

        virtual public void Subscribe()
        {
            //MessageService.Subscribe<ILoginService, bool>(this, OnLoginMessage);
            messageService.Subscribe<ViewModelBase, string>(this, OnMessage);
        }

        virtual public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
        }

        private async void OnLoginMessage(
            //ILoginService loginService,
            string message,
            bool isAuthenticated)
        {
            if (message == "AuthenticationChanged")
            {
                await contextService.RunAsync(() =>
                {
                    IsLocked = !isAuthenticated;
                });
            }
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
    }
}
