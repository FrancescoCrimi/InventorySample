using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Inventory.UwpApp.Core.Models;
using Inventory.UwpApp.Core.Services;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Inventory.UwpApp.ViewModels
{
    public class DataGridViewModel : ObservableObject
    {
        public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

        public DataGridViewModel()
        {
        }

        public async Task LoadDataAsync()
        {
            Source.Clear();

            // Replace this with your actual data
            var data = await SampleDataService.GetGridDataAsync();

            foreach (var item in data)
            {
                Source.Add(item);
            }
        }
    }
}
