using CADToolBox.Modules.TrackerGA.Messages;
using CADToolBox.Shared.Models.CADModels.Implement;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CADToolBox.Modules.TrackerGA.ViewModels.SubViewModels;

public partial class SummaryViewModel : ViewModelBase {
    [ObservableProperty]
    private TrackerModel? _trackerModel;

    public SummaryViewModel() {
        TrackerModel = TrackerApp.Current.TrackerModel;
    }

    [RelayCommand]
    private void SaveOnly() {
        TrackerModel!.Status = 0;
        WeakReferenceMessenger.Default.Send(new WindowCloseMessage());
    }

    [RelayCommand]
    private void DrawOnly() {
        TrackerModel!.Status = 1;
        WeakReferenceMessenger.Default.Send(new WindowCloseMessage());
    }

    [RelayCommand]
    private void SaveAndDraw() {
        TrackerModel!.Status = 2;
        WeakReferenceMessenger.Default.Send(new WindowCloseMessage());
    }
}