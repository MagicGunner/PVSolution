using System;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using SapToolBox.Base.Common.Interface;
using SapToolBox.Base.MVVM;

namespace SapToolBox.PreTools.ViewModels.SubViewModels {
    public class ModelSupportViewModel : NavigationViewModel {
    #region 字段属性

        private readonly IContainerProvider _provider;
        private readonly IRegionManager     _regionManager;

        private string _textStr = "前处理建模辅助(正在施工中 喵~~)";

        public string TextStr {
            get => _textStr;
            set => SetProperty(ref _textStr, value);
        }

    #endregion

    #region 构造函数

        public ModelSupportViewModel() {
        }

        public ModelSupportViewModel(IContainerProvider provider,
                                     IRegionManager     regionManager) {
            _provider      = provider;
            _regionManager = regionManager;
        }

    #endregion


    #region 委托声明

    #endregion

    #region 委托实现

    #endregion

    #region 方法

    #endregion
    }
}

