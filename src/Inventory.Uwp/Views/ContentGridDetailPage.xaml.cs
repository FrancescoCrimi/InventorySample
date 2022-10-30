using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Inventory.Uwp.Views
{
    public sealed partial class ContentGridDetailPage : Page
    {
        public ContentGridDetailViewModel ViewModel { get; } = Ioc.Default.GetService<ContentGridDetailViewModel>();

        public ContentGridDetailPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.RegisterElementForConnectedAnimation("animationKeyContentGrid", itemHero);
            if (e.Parameter is long orderID)
            {
                await ViewModel.InitializeAsync(orderID);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                Ioc.Default.GetService<NavigationService>().Frame.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
            }
        }
    }
}
