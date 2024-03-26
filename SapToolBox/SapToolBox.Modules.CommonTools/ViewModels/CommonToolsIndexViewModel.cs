using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using SapToolBox.Modules.CommonTools.ViewModels.SubViewModels;
using SapToolBox.Modules.CommonTools.Views.SubViews;
using SapToolBox.Shared.Models.UIModels.Implement;
using SapToolBox.Shared.MVVM;
using SapToolBox.Shared.Prism;

namespace SapToolBox.Modules.CommonTools.ViewModels;

public partial class CommonToolsIndexViewModel : NavigationViewModel {
#region 字段属性

    private readonly IContainerProvider _containerProvider;
    private readonly IRegionManager     _regionManager;

    // 初始化完成标志
    private bool _inited;

    public bool InitFlag {
        get => _inited;
        set => SetProperty(ref _inited, value);
    }

    private ObservableCollection<MenuBar> _menuBars = [
                                                          new MenuBar {
                                                                          Icon      = "Number1",
                                                                          Title     = "首页",
                                                                          NameSpace = nameof(HomeView)
                                                                      },
                                                          new MenuBar {
                                                                          Icon      = "Number2",
                                                                          Title     = "截面批量导入",
                                                                          NameSpace = nameof(SectionDefView)
                                                                      }
                                                      ];

    public ObservableCollection<MenuBar> MenuBars {
        get => _menuBars;
        set => SetProperty(ref _menuBars, value);
    }

#endregion

#region 构造函数

    public CommonToolsIndexViewModel(IContainerProvider containerProvider,
                                     IRegionManager     regionManager) {
        _containerProvider = containerProvider;
        _regionManager     = regionManager;

        SelectedIndexChangedCommand = new DelegateCommand<MenuBar>(SelectedIndexChanged);
    }

#endregion

#region 委托声明

    /// <summary>
    /// 页面切换
    /// </summary>
    public DelegateCommand<MenuBar> SelectedIndexChangedCommand { get; private set; }

#endregion


#region 委托实现

    private void SelectedIndexChanged(MenuBar obj) {
        try {
            _regionManager.RequestNavigate(PrismManager.CommonToolsViewRegionName, obj.NameSpace);
        } catch { // ignored
        }
    }

#endregion

#region 方法区

    public override void OnNavigatedTo(NavigationContext navigationContext) {
        // 第一次导航至该页面时跳转到初始页面
        if (_inited) return;
        _inited = true;
        _regionManager.RequestNavigate(PrismManager.CommonToolsViewRegionName,
                                       nameof(HomeView),
                                       back => {
                                           if (back.Error != null) { }
                                       });
    }

#endregion
}