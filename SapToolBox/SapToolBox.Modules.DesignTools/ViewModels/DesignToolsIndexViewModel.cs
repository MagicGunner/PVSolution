using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using SapToolBox.Modules.DesignTools.Views.SubViews;
using SapToolBox.Shared.Models.UIModels.Implement;
using SapToolBox.Shared.MVVM;
using SapToolBox.Shared.Prism;

namespace SapToolBox.Modules.DesignTools.ViewModels {
    public class DesignToolsIndexViewModel : NavigationViewModel {
    #region 字段与属性

        private readonly IContainerProvider? _containerProvider;
        private readonly IRegionManager      _regionManager;

        private ObservableCollection<MenuBar> _menuBars = [
                                                              new MenuBar {
                                                                              Icon      = "Number1",
                                                                              Title     = "截面设计辅助",
                                                                              NameSpace = "SectionDesignerView"
                                                                          },
                                                              //new MenuBar {
                                                              //                Icon      = "Number2",
                                                              //                Title     = "截面设计辅助",
                                                              //                NameSpace = "DesignOverwriteView"
                                                              //            }
                                                          ];

        public ObservableCollection<MenuBar> MenuBars {
            get => _menuBars;
            set => SetProperty(ref _menuBars, value);
        }

        // 初始化完成标志
        private bool _inited;

        public bool InitFlag {
            get => _inited;
            set => SetProperty(ref _inited, value);
        }

        //public string                      Title { get; }
        //public event Action<IDialogResult> RequestClose;
        public string? DialogHostName { get; set; }

    #endregion

    #region 构造函数

        public DesignToolsIndexViewModel(IContainerProvider containerProvider,
                                         IRegionManager     regionManager) {
            _containerProvider          = containerProvider;
            _regionManager              = regionManager;
            SelectedIndexChangedCommand = new DelegateCommand<MenuBar>(SelectedIndexChanged);
            SaveCommand                 = new DelegateCommand(Save);
            CancelCommand               = new DelegateCommand(Cancel);
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
                _regionManager.RequestNavigate(PrismManager.DesignToolsViewRegionName, obj.NameSpace);
            } catch { // ignored
            }
        }

        private void Save() {
        }

        private void Cancel() {
        }

    #endregion

    #region 方法

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            // 第一次导航至该页面时跳转到初始页面
            if (_inited) return;
            _inited = true;
            _regionManager.RequestNavigate(PrismManager.DesignToolsViewRegionName,
                                           nameof(SectionDesignerView),
                                           back => {
                                               if (back.Error != null) { }
                                           });
        }

    #endregion
    }
}