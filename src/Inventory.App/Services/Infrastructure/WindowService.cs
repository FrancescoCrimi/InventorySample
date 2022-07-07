using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace CiccioSoft.Inventory.Uwp.Services.Infrastructure
{
    public class WindowService
    {
        private readonly ILogger<WindowService> logger;
        private readonly PageService pageService;

        public WindowService(ILogger<WindowService> logger,
                                    PageService pageService)
        {
            this.logger = logger;
            this.pageService = pageService;
        }

        public Dictionary<UIContext, AppWindow> AppWindows { get; set; }
            = new Dictionary<UIContext, AppWindow>();

        public async Task<int> OpenInNewWindow<TViewModel>(object parameter = null)
        {
            return await OpenInNewWindow(typeof(TViewModel), parameter);
        }

        public async Task<int> OpenInNewWindow(Type viewModelType, object parameter = null)
        {
            AppWindow appWindow = await AppWindow.TryCreateAsync();
            Frame appWindowFrame = new Frame();
            var view = pageService.GetView(viewModelType);
            appWindowFrame.Navigate(view, parameter);
            ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowFrame);
            AppWindows.Add(appWindowFrame.UIContext, appWindow);
            appWindow.Closed += delegate
            {
                AppWindows.Remove(appWindowFrame.UIContext);
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
