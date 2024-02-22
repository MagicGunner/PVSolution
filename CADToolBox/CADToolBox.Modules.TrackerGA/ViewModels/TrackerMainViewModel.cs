using System.Collections.ObjectModel;
using System.Windows;
using CADToolBox.Modules.TrackerGA.Services.Implement;
using CADToolBox.Modules.TrackerGA.ViewModels.SubViewModels;
using CADToolBox.Shared.Models.UIModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CADToolBox.Modules.TrackerGA.ViewModels;

public partial class TrackerMainViewModel : ViewModelBase {
#region 字段与属性

    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    private ObservableCollection<MenuBar>? _menuBars;

    [ObservableProperty]
    private string? _userName = "MissBlue";

#endregion

#region 构造函数

    public TrackerMainViewModel(NavigationService navigationService) {
        MenuBars = [];
        CreateMenuBars();

        _navigationService                         =  navigationService;
        _navigationService.CurrentViewModelChanged += () => { CurrentViewModel = navigationService.CurrentViewModel; };
        _navigationService.NavigateTo<HomeViewModel>();
    }

#endregion

#region 普通方法

    private void CreateMenuBars() {
        MenuBars?.Add(new MenuBar { Icon = "Number1", Title = "首页" });
        MenuBars?.Add(new MenuBar { Icon = "Number2", Title = "设计信息" });
        MenuBars?.Add(new MenuBar { Icon = "Number3", Title = "跨距信息" });
        MenuBars?.Add(new MenuBar { Icon = "Number4", Title = "信息汇总" });
    }

#endregion

#region RelayCommand

    [RelayCommand]
    private void SelectedIndexChanged(MenuBar obj) {
        switch (obj.Title) {
            case "首页":
                _navigationService.NavigateTo<HomeViewModel>();
                break;
            case "设计信息":
                _navigationService.NavigateTo<DesignInfoViewModel>();
                break;
            case "跨距信息":
                _navigationService.NavigateTo<SpanInfoViewModel>();
                break;
            case "信息汇总":
                _navigationService.NavigateTo<SummaryViewModel>();
                break;
        }
    }

#endregion
}