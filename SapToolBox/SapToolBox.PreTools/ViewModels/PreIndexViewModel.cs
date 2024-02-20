﻿using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using SapToolBox.Base;
using SapToolBox.Base.Common;
using SapToolBox.Base.Common.Interface;
using SapToolBox.Base.MVVM;
using SapToolBox.PreTools.Views.SubViews;
using SapToolBox.Shared.Models;

namespace SapToolBox.PreTools.ViewModels {
    public class PreIndexViewModel : NavigationViewModel {
    #region 字段与属性

        private readonly IContainerProvider _provider;
        private readonly IRegionManager     _regionManager;

        private ObservableCollection<MenuBar> _menuBars = [
                                                              new MenuBar() {
                                                                                Icon      = "Number1", Title = "设计覆盖项",
                                                                                NameSpace = "DesignOverwriteView"
                                                                            },
                                                              new MenuBar() {
                                                                                Icon      = "Number2", Title = "建模辅助",
                                                                                NameSpace = "ModelSupportView"
                                                                            },
                                                              new MenuBar() {
                                                                                Icon      = "Number3", Title = "截面设计辅助",
                                                                                NameSpace = "SectionDesignerView"
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

        //public string                      Title { get; }
        //public event Action<IDialogResult> RequestClose;
        public string DialogHostName { get; set; }

    #endregion

    #region 构造函数

        public PreIndexViewModel() {
        }

        public PreIndexViewModel(IContainerProvider provider, IRegionManager regionManager) {
            _provider                   = provider;
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
                _regionManager.RequestNavigate(PrismManager.PreIndexViewRegionName, obj.NameSpace);
            }
            catch { // ignored
            }
        }

        private void Save() {
        }

        private void Cancel() {
        }

    #endregion

    #region 方法

        public bool CanCloseDialog() {
            throw new NotImplementedException();
        }

        public void OnDialogClosed() {
            throw new NotImplementedException();
        }


        public void OnDialogOpened(IDialogParameters parameters) {
            throw new NotImplementedException();
        }


        public override void OnNavigatedTo(NavigationContext navigationContext) {
            // 第一次导航至该页面时跳转到初始页面
            if (_Inited) return;
            _Inited = true;
            _regionManager.RequestNavigate(PrismManager.PreIndexViewRegionName, nameof(SectionDesignerView), back => {
                                               if (back.Error != null) {
                                               }
                                           });
        }

    #endregion
    }
}