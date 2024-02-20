using CADToolBox.Modules.TrackerGA.Services.Implement;

namespace CADToolBox.Modules.TrackerGA.ViewModels.SubViewModels;

public class HomeViewModel : ViewModelBase {
    private readonly NavigationService _navigationService;

    public HomeViewModel(NavigationService navigationService) {
        _navigationService = navigationService;
    }
}