﻿#region copyright
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
using System.Windows.Input;
using System.Threading.Tasks;

using CiccioSoft.Inventory.Uwp.Services;
using Microsoft.Toolkit.Mvvm.Input;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;
        private readonly IDialogService dialogService;

        public LoginViewModel(INavigationService navigationService,
                              //ILoginService loginService,
                              ISettingsService settingsService,
                              IDialogService dialogService)
            : base()
        {
            this.navigationService = navigationService;
            //LoginService = loginService;
            SettingsService = settingsService;
            this.dialogService = dialogService;
        }

        public ILoginService LoginService { get; }
        public ISettingsService SettingsService { get; }

        private ShellArgs ViewModelArgs { get; set; }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private bool _isLoginWithPassword = false;
        public bool IsLoginWithPassword
        {
            get { return _isLoginWithPassword; }
            set { SetProperty(ref _isLoginWithPassword, value); }
        }

        private bool _isLoginWithWindowsHello = false;
        public bool IsLoginWithWindowsHello
        {
            get { return _isLoginWithWindowsHello; }
            set { SetProperty(ref _isLoginWithWindowsHello, value); }
        }

        private string _userName = null;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private string _password = "UserPassword";

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public ICommand ShowLoginWithPasswordCommand => new RelayCommand(ShowLoginWithPassword);
        public ICommand LoginWithPasswordCommand => new RelayCommand(LoginWithPassword);
        public ICommand LoginWithWindowHelloCommand => new RelayCommand(LoginWithWindowHello);

        public Task LoadAsync(ShellArgs args)
        {
            ViewModelArgs = args;

            UserName = SettingsService.UserName ?? args.UserInfo.AccountName;
            IsLoginWithWindowsHello = LoginService.IsWindowsHelloEnabled(UserName);
            IsLoginWithPassword = !IsLoginWithWindowsHello;
            IsBusy = false;

            return Task.CompletedTask;
        }

        public void Login()
        {
            if (IsLoginWithPassword)
            {
                LoginWithPassword();
            }
            else
            {
                LoginWithWindowHello();
            }
        }

        private void ShowLoginWithPassword()
        {
            IsLoginWithWindowsHello = false;
            IsLoginWithPassword = true;
        }

        public async void LoginWithPassword()
        {
            IsBusy = true;
            var result = ValidateInput();
            if (result.IsOk)
            {
                if (await LoginService.SignInWithPasswordAsync(UserName, Password))
                {
                    if (!LoginService.IsWindowsHelloEnabled(UserName))
                    {
                        await LoginService.TrySetupWindowsHelloAsync(UserName);
                    }
                    SettingsService.UserName = UserName;
                    EnterApplication();
                    return;
                }
            }
            await dialogService.ShowAsync(result.Message, result.Description);
            IsBusy = false;
        }

        public async void LoginWithWindowHello()
        {
            IsBusy = true;
            var result = await LoginService.SignInWithWindowsHelloAsync();
            if (result.IsOk)
            {
                EnterApplication();
                return;
            }
            await dialogService.ShowAsync(result.Message, result.Description);
            IsBusy = false;
        }

        private void EnterApplication()
        {
            if (ViewModelArgs.UserInfo.AccountName != UserName)
            {
                ViewModelArgs.UserInfo = new UserInfo
                {
                    AccountName = UserName,
                    FirstName = UserName,
                    PictureSource = null
                };
            }
            navigationService.Navigate<MainShellViewModel>(ViewModelArgs);
        }

        private Result ValidateInput()
        {
            if (String.IsNullOrWhiteSpace(UserName))
            {
                return Result.Error("Login error", "Please, enter a valid user name.");
            }
            if (String.IsNullOrWhiteSpace(Password))
            {
                return Result.Error("Login error", "Please, enter a valid password.");
            }
            return Result.Ok();
        }
    }
}