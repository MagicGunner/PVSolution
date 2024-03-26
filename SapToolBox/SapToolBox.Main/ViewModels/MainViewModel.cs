using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using SapToolBox.Main.Common.Interface;
using SapToolBox.Modules.DesignTools.Views;
using SapToolBox.Shared.Models.UIModels.Implement;
using SapToolBox.Shared.Prism;

namespace SapToolBox.Main.ViewModels;

public class MainViewModel : BindableBase, IConfigureService {
    #region 字段和属性

    private string? _userName;

    public string? UserName {
        get => _userName;
        set {
            _userName = value;
            RaisePropertyChanged();
        }
    }

    private ObservableCollection<MenuBar>? _menuBars;

    public ObservableCollection<MenuBar>? MenuBars {
        get => _menuBars;
        set => SetProperty(ref _menuBars, value);
    }

    // 委托函数
    public DelegateCommand<MenuBar> NavigateCommand  { get; private set; }
    public DelegateCommand          GoBackCommand    { get; private set; }
    public DelegateCommand          GoForwardCommand { get; private set; }

    private          IRegionNavigationJournal _journal;
    private readonly IContainerProvider       _containerProvider;
    private readonly IRegionManager           _regionManager;

    #endregion


    #region 构造方法区

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="containerProvider"></param>
    /// <param name="regionManager"></param>
    public MainViewModel(IContainerProvider containerProvider,
                         IRegionManager     regionManager) {
        MenuBars = [];
        NavigateCommand = new DelegateCommand<MenuBar>(Navigate);

        GoBackCommand = new DelegateCommand(() => {
                                                if (_journal is { CanGoBack: true }) {
                                                    _journal.GoBack();
                                                }
                                            });
        GoForwardCommand = new DelegateCommand(() => {
                                                   if (_journal is { CanGoForward: true }) {
                                                       _journal.GoForward();
                                                   }
                                               });

        _containerProvider = containerProvider;
        _regionManager = regionManager;
    }

    #endregion

    #region 方法区

    private void Navigate(MenuBar obj) {
        if (obj != null && string.IsNullOrWhiteSpace(obj.NameSpace)) {
            _regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.NameSpace, back => { _journal = back.Context.NavigationService.Journal; });
        }
    }

    public void Configure() {
        UserName = "MissBlue";
        CreateMenuBar();
        _regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(nameof(DesignToolsIndexView));
    }


    private void CreateMenuBar() {
        MenuBars?.Add(new MenuBar() {
                                        Icon = "Home",
                                        Title = "首页",
                                        NameSpace = "IndexView"
                                    });
        MenuBars?.Add(new MenuBar() {
                                        Icon = "NotebookOutline",
                                        Title = "设计工具",
                                        NameSpace = nameof(DesignToolsIndexView)
                                    });
        MenuBars?.Add(new MenuBar() {
                                        Icon = "NotebookPlus",
                                        Title = "后处理页",
                                        NameSpace = "PostToolsView"
                                    });
        MenuBars?.Add(new MenuBar() {
                                        Icon = "Cog",
                                        Title = "设置",
                                        NameSpace = "SettingsView"
                                    });
    }

    #endregion
}