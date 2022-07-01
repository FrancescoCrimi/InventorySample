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

using Microsoft.Extensions.Logging;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;

namespace CiccioSoft.Inventory.Uwp.Services.Infrastructure.Impl
{
    public partial class NavigationService : INavigationService
    {
        private readonly ILogger<NavigationService> logger;
        private readonly PageService pageService;

        public NavigationService(ILogger<NavigationService> logger,
                                 PageService pageService)
        {
            this.logger = logger;
            this.pageService = pageService;
        }

        static public int MainViewId { get; }

        public bool IsMainView => CoreApplication.GetCurrentView().IsMain;

        public Frame Frame { get; private set; }

        public bool CanGoBack => Frame.CanGoBack;

        public void GoBack() => Frame.GoBack();

        public void Initialize(object frame)
        {
            Frame = frame as Frame;
        }

        public bool Navigate<TViewModel>(object parameter = null)
        {
            return Navigate(typeof(TViewModel), parameter);
        }
        public bool Navigate(Type viewModelType, object parameter = null)
        {
            if (Frame == null)
            {
                throw new InvalidOperationException("Navigation frame not initialized.");
            }
            return Frame.Navigate(pageService.GetView(viewModelType), parameter);
        }
    }
}
