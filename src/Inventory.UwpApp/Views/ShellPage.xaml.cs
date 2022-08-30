using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.UwpApp.ViewModels;
using System;
using Windows.UI.Xaml.Controls;

namespace Inventory.UwpApp.Views
{
    // TODO: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = Ioc.Default.GetService<ShellViewModel>();

        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);
        }
    }
}
