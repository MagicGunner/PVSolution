using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using SapToolBox.Shared.Helpers;
using SapToolBox.Shared.MVVM;
using SapToolBox.Shared.Sap2000;

namespace SapToolBox.Modules.DesignTools.ViewModels.SubViewModels {
    public class DesignOverwriteViewModel : NavigationViewModel {
    #region 字段属性

        private SapModelHelper _currentSapModelHelper;

        public SapModelHelper CurrentSapModelHelper {
            get => _currentSapModelHelper;
            set => SetProperty(ref _currentSapModelHelper, value);
        }

        private ObservableCollection<String> _selectedGroupNames;

        public ObservableCollection<string> SelectedGroupNames {
            get => _selectedGroupNames;
            set => SetProperty(ref _selectedGroupNames, value);
        }

        private readonly IContainerProvider _provider;
        private readonly IRegionManager     _regionManager;


        // 设计覆盖项
        private BindingList<OverWriteObj> _overWrites;

        public BindingList<OverWriteObj> OverWrites {
            get => _overWrites;
            set => SetProperty(ref _overWrites, value);
        }

    #endregion

    #region 构造函数

        public DesignOverwriteViewModel(IContainerProvider provider,
                                        IRegionManager     regionManager) {
            _currentSapModelHelper = provider.Resolve<SapModelHelper>();
            _provider = provider;
            _regionManager = regionManager;


            SetOverwriteCommand = new DelegateCommand<IList>(SetOverwrite);

            //OverWrites = new BindingList<OverWriteObj>(_currentSapModelHelper.OverWriteObjs);
        }

    #endregion


    #region 委托声明

        public DelegateCommand<IList> SetOverwriteCommand { get; set; }

    #endregion

    #region 委托实现

        private void SetOverwrite(IList groupList) {
            if (groupList.Count == 0) {
                foreach (var item in OverWrites) {
                    if (item.NeedModify) {
                        _currentSapModelHelper.SetOverwrite(item.Index, item.Value);
                    }
                }
            } else {
                foreach (var groupName in groupList) {
                    foreach (var item in OverWrites) {
                        if (item.NeedModify) {
                            _currentSapModelHelper.SetOverwrite(groupName.ToString(), item.Index, item.Value);
                        }
                    }
                }
            }
        }

    #endregion
    }
}