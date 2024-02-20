using System.Collections.ObjectModel;
using CADToolBox.Modules.TrackerGA.Services.Implement;
using CADToolBox.Modules.TrackerGA.ViewModels.SubViewModels;
using CADToolBox.Shared.Models.UIModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Modules.TrackerGA.ViewModels;

public partial class TrackerMainViewModel : ViewModelBase {
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    private ObservableCollection<MenuBar>? _menuBars;

    [ObservableProperty]
    private string? _userName = "MissBlue";

    public TrackerMainViewModel(NavigationService navigationService) {
        MenuBars = [];
        CreateMenuBars();

        _navigationService                         =  navigationService;
        _navigationService.CurrentViewModelChanged += () => { CurrentViewModel = navigationService.CurrentViewModel; };
        _navigationService.NavigateTo<HomeViewModel>();
    }

    private void CreateMenuBars() {
        MenuBars?.Add(new MenuBar { Icon = "Home", Title = "首页" });
        MenuBars?.Add(new MenuBar { Icon = "Home", Title = "设计信息" });
        MenuBars?.Add(new MenuBar { Icon = "Home", Title = "跨距信息" });
        MenuBars?.Add(new MenuBar { Icon = "Cog", Title  = "信息汇总" });
    }
}