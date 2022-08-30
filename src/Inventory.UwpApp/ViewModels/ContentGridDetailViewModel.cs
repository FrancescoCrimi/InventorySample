using System;
using System.Linq;
using System.Threading.Tasks;

using Inventory.UwpApp.Core.Models;
using Inventory.UwpApp.Core.Services;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Inventory.UwpApp.ViewModels
{
    public class ContentGridDetailViewModel : ObservableObject
    {
        private SampleOrder _item;

        public SampleOrder Item
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }

        public ContentGridDetailViewModel()
        {
        }

        public async Task InitializeAsync(long orderID)
        {
            var data = await SampleDataService.GetContentGridDataAsync();
            Item = data.First(i => i.OrderID == orderID);
        }
    }
}
