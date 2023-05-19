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

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Inventory.Uwp.Services
{
    public class WindowManagerService
    {
        private readonly ILogger<WindowManagerService> logger;
        private readonly Dictionary<UIContext, AppWindow> appWindows;

        public WindowManagerService(ILogger<WindowManagerService> logger)
        {
            this.logger = logger;
            appWindows = new Dictionary<UIContext, AppWindow>();
        }

        public async Task<int> OpenInNewWindow<TView>(object parameter = null)
        {
            return await OpenInNewWindow(typeof(TView), parameter);
        }

        public async Task<int> OpenInNewWindow(Type viewType, object parameter = null)
        {
            AppWindow appWindow = await AppWindow.TryCreateAsync();
            Frame appWindowFrame = new Frame();
            appWindowFrame.Navigate(viewType, parameter);
            ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowFrame);
            appWindows.Add(appWindowFrame.UIContext, appWindow);
            appWindow.Closed += delegate
            {
                appWindows.Remove(appWindowFrame.UIContext);
                appWindowFrame.Content = null;
                appWindow = null;
            };
            await appWindow.TryShowAsync();
            return 0;
        }

        public async Task CloseViewAsync()
        {
            //int currentId = ApplicationView.GetForCurrentView().Id;
            //await ApplicationViewSwitcher.SwitchAsync(MainViewId, currentId, ApplicationViewSwitchingOptions.ConsolidateViews);
            await Task.CompletedTask;
        }
    }
}
