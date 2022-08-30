using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.UwpApp.ViewModels;
using System;
using Windows.UI.Xaml.Controls;

namespace Inventory.UwpApp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = Ioc.Default.GetService<MainViewModel>();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
