using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.UwpApp.ViewModels;
using System;
using Windows.UI.Xaml.Controls;

namespace Inventory.UwpApp.Views
{
    // TODO: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            InitializeComponent();
            ShellViewModel viewModel = Ioc.Default.GetService<ShellViewModel>();
            DataContext = viewModel;
            viewModel.Initialize(shellFrame, KeyboardAccelerators);
        }
    }
}
