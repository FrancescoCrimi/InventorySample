using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using Inventory.WinUi.Contracts.ViewModels;
using Inventory.WinUi.Core.Contracts.Services;
using Inventory.WinUi.Core.Models;

namespace Inventory.WinUi.ViewModels;

public class DataGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

    public DataGridViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        var data = await _sampleDataService.GetGridDataAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
