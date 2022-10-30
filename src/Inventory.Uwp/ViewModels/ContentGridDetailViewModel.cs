using Inventory.Uwp.Models;
using Inventory.Uwp.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels
{
    public class ContentGridDetailViewModel : Models.ObservableObject
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
