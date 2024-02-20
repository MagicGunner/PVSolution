using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using SapToolBox.Base;
using SapToolBox.Base.MVVM;
using SapToolBox.PreTools.Views.SubViews;
using SapToolBox.Shared.Models;
using System.Collections.ObjectModel;
using System;
using SapToolBox.PreTools.Views.SubViews.SectionViews;

namespace SapToolBox.PreTools.ViewModels.SubViewModels;

public class SectionDesignerViewModel : NavigationViewModel {
#region 字段与属性

    private readonly IContainerProvider _provider;
    private readonly IRegionManager     _regionManager;

    private ObservableCollection<MenuBar> _menuBars = [
                                                          new MenuBar() {
                                                                            Icon      = "Number1",
                                                                            Title     = "H型钢",
                                                                            NameSpace = "HSectionView"
                                                                        },
                                                          new MenuBar() {
                                                                            Icon      = "Number2",
                                                                            Title     = "C型钢",
                                                                            NameSpace = "CSectionView"
                                                                        }
                                                      ];

    public ObservableCollection<MenuBar> MenuBars {
        get => _menuBars;
        set => SetProperty(ref _menuBars, value);
    }

    // 初始化完成标志
    private bool _Inited;

    public bool InitFlag {
        get => _Inited;
        set => SetProperty(ref _Inited, value);
    }

#endregion

#region 构造函数

    public SectionDesignerViewModel() {
    }

    public SectionDesignerViewModel(IContainerProvider provider, IRegionManager regionManager) {
        _provider                   = provider;
        _regionManager              = regionManager;
        SelectedIndexChangedCommand = new DelegateCommand<MenuBar>(SelectedIndexChanged);
    }

#endregion

#region 委托声明

    /// <summary>
    /// 页面切换
    /// </summary>
    public DelegateCommand<MenuBar> SelectedIndexChangedCommand { get; private set; }

    public DelegateCommand SaveCommand   { get; set; }
    public DelegateCommand CancelCommand { get; set; }

#endregion

#region 委托实现

    private void SelectedIndexChanged(MenuBar obj) {
        try {
            _regionManager.RequestNavigate(PrismManager.SectionDesignerViewRegionName, obj.NameSpace);
        } catch { // ignored
        }
    }

#endregion

#region 方法

    public override void OnNavigatedTo(NavigationContext navigationContext) {
        // 第一次导航至该页面时跳转到初始页面
        if (_Inited) return;
        _Inited = true;
        _regionManager.RequestNavigate(PrismManager.SectionDesignerViewRegionName,
                                       nameof(CSectionView),
                                       back => {
                                           if (back.Error != null) { }
                                       });
    }

#endregion
}