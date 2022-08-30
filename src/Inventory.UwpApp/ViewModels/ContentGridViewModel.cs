using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Inventory.UwpApp.Core.Models;
using Inventory.UwpApp.Core.Services;
using Inventory.UwpApp.Services;
using Inventory.UwpApp.Views;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Animations;

namespace Inventory.UwpApp.ViewModels
{
    public class ContentGridViewModel : ObservableObject
    {
        private ICommand _itemClickCommand;
        private readonly NavigationService navigationService;

        public ICommand ItemClickCommand => _itemClickCommand ?? (_itemClickCommand = new RelayCommand<SampleOrder>(OnItemClick));

        public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

        public ContentGridViewModel(NavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public async Task LoadDataAsync()
        {
            Source.Clear();

            // Replace this with your actual data
            var data = await SampleDataService.GetContentGridDataAsync();
            foreach (var item in data)
            {
                Source.Add(item);
            }
        }

        private void OnItemClick(SampleOrder clickedItem)
        {
            if (clickedItem != null)
            {
                navigationService.Frame.SetListDataItemForNextConnectedAnimation(clickedItem);
                navigationService.Navigate<ContentGridDetailPage>(clickedItem.OrderID);
            }
        }
    }
}
