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

using CiccioSoft.Inventory.Uwp.Services;
//using Microsoft.Extensions.Uwp.Logging;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    public class ShellArgs
    {
        public Type ViewModel { get; set; }
        public object Parameter { get; set; }
        public UserInfo UserInfo { get; set; }
    }

    public class ShellViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;

        public ShellViewModel(
                              //ILoginService loginService,
                              INavigationService navigationService)
            : base()
        {
            //IsLocked = !loginService.IsAuthenticated;
            this.navigationService = navigationService;
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
            //MessageService.Subscribe<ViewModelBase, string>(this, OnMessage);
            Messenger.Register<StatusMessage>(this, OnStatusMessage);
        }

        virtual public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
        }

        //private async void OnLoginMessage(ILoginService loginService, string message, bool isAuthenticated)
        //{
        //    if (message == "AuthenticationChanged")
        //    {
        //        //await ContextService.RunAsync(() =>
        //        //{
        //        IsLocked = !isAuthenticated;
        //        //});
        //    }
        //}

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

        private void SetStatus(string message)
        {
            message = message ?? "";
            message = message.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
            Message = message;
        }
    }
}
