using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace CiccioSoft.Inventory.Uwp.Services.Infrastructure.Impl
{
    public class WindowService : IWindowService
    {
        private readonly ILogger<WindowService> logger;
        private readonly PageService pageService;

        public WindowService(ILogger<WindowService> logger,
                                    PageService pageService)
        {
            this.logger = logger;
            this.pageService = pageService;
        }

        public bool? OpenInDialog(Type viewModelType, object parameter = null)
        {
            throw new NotImplementedException();
        }


        public async Task<int> OpenInNewWindow<TViewModel>(object parameter = null)
        {
            return await OpenInNewWindow(typeof(TViewModel), parameter);
        }
        public async Task<int> OpenInNewWindow(Type viewModelType, object parameter = null)
        {
            // New Implementation in AppWindow
            Frame appWindowFrame = new Frame();
            // Create a new window
            AppWindow appWindow = await AppWindow.TryCreateAsync();
            // Make sure we release the reference to this window, and release XAML resources, when it's closed
            appWindow.Closed += delegate { appWindow = null; appWindowFrame.Content = null; };
            var view = pageService.GetView(viewModelType);
            // Navigate the frame to the page we want to show in the new window
            appWindowFrame.Navigate(view, parameter);
            // Attach the XAML content to our window
            ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowFrame);
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
