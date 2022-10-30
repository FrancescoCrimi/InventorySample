using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Uwp.ViewModels;
using System;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views
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
