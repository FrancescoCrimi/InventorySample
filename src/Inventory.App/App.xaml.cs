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

using CiccioSoft.Inventory.Uwp.ViewModels;
using CiccioSoft.Inventory.Uwp.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CiccioSoft.Inventory.Uwp
{
    sealed partial class App : Windows.UI.Xaml.Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                Frame rootFrame = new Frame();
                var startup = new Startup();
                await startup.ConfigureAsync();

                var shellArgs = new ShellArgs
                {
                    ViewModel = typeof(DashboardViewModel),
                    Parameter = null,
                    UserInfo = await TryGetUserInfoAsync(e as IActivatedEventArgsWithUser)
                };

                rootFrame.Navigate(typeof(MainShellView), shellArgs);

                Window.Current.Content = rootFrame;
                //Window.Current.Activate();
            }

            if (e.PrelaunchActivated == false)
            {
                Window.Current.Activate();
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var logger = Ioc.Default.GetService<ILogger<App>>();
            logger.LogInformation($"Application ended by '{AppSettings.Current.UserName}'.");
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            var logger = Ioc.Default.GetService<ILogger<App>>();
            logger.LogError("UnhandledException: " + e.Message);
        }

        private async Task<UserInfo> TryGetUserInfoAsync(IActivatedEventArgsWithUser argsWithUser)
        {
            if (argsWithUser != null)
            {
                var user = argsWithUser.User;
                var userInfo = new UserInfo
                {
                    AccountName = await user.GetPropertyAsync(KnownUserProperties.AccountName) as String,
                    FirstName = await user.GetPropertyAsync(KnownUserProperties.FirstName) as String,
                    LastName = await user.GetPropertyAsync(KnownUserProperties.LastName) as String
                };
                if (!userInfo.IsEmpty)
                {
                    if (String.IsNullOrEmpty(userInfo.AccountName))
                    {
                        userInfo.AccountName = $"{userInfo.FirstName} {userInfo.LastName}";
                    }
                    var pictureStream = await user.GetPictureAsync(UserPictureSize.Size64x64);
                    if (pictureStream != null)
                    {
                        userInfo.PictureSource = await BitmapTools.LoadBitmapAsync(pictureStream);
                    }
                    return userInfo;
                }
            }
            return UserInfo.Default;
        }
    }
}
