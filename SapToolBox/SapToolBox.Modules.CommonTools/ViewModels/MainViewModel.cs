using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapToolBox.Modules.CommonTools.Services.Implement;
using SapToolBox.Modules.CommonTools.ViewModels.SubViewModels;
using SapToolBox.Shared.Models.UIModels.Implement;

namespace SapToolBox.Modules.CommonTools.ViewModels;

public partial class MainViewModel : ViewModelBase {
    #region 字段属性

    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    private string? _userName = "MissBlue";

    [ObservableProperty]
    private ObservableCollection<MenuBar>? _menuBars;

    #endregion


    public MainViewModel(NavigationService navigationService) {
        MenuBars = [];
        CreateMenuBars();
        _navigationService = navigationService;
        _navigationService.CurrentViewModelChanged += () => { CurrentViewModel = navigationService.CurrentViewModel; };
        _navigationService.NavigateTo<HomeViewModel>();
    }

    #region 普通方法区

    private void CreateMenuBars() {
        MenuBars?.Add(new MenuBar {
                                      Icon = "Number1",
                                      Title = "首页"
                                  });
        MenuBars?.Add(new MenuBar {
                                      Icon = "Number2",
                                      Title = "截面定义辅助"
                                  });
    }

    #endregion

    #region RelayCommand

    [RelayCommand]
    private void SelectedIndexChanged(MenuBar obj) {
        switch (obj.Title) {
            case "首页":
                _navigationService.NavigateTo<HomeViewModel>();
                break;
            case "截面定义辅助":
                _navigationService.NavigateTo<SectionDefViewModel>();
                break;
        }
    }

    #endregion
}