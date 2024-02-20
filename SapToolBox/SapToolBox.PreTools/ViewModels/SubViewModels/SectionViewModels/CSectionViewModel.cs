using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Prism.Mvvm;
using SapToolBox.Base.DesignCodes;
using SapToolBox.Base.DesignCodes.Implement;
using SapToolBox.Base.Sections;
using SapToolBox.Base.Sections.Implement;

namespace SapToolBox.PreTools.ViewModels.SubViewModels.SectionViewModels;

public class CSectionViewModel : SectionViewModelBase {
    private CSection _currentSection;

    public CSection CurrentSection {
        get => _currentSection;
        set => SetProperty(ref _currentSection, value);
    }


    private ChineseCode _currentDesignModel;

    public ChineseCode CurrentDesignModel {
        get => _currentDesignModel;
        set => SetProperty(ref _currentDesignModel, value);
    }

    public CSectionViewModel() {
        CurrentSection = new CSection("C210x100x30x3.0", 210, 100, 30, 3.0, 3.0);
        CurrentDesignModel = new ChineseCode(CurrentSection, 210000, 445, 355, 305, 175, 1000, 1.0, 1.0, 1.0, false,
                                             false, 1.0, 30, 15);
        CurrentSection.PropertyChanged += OnSectionPropertyChanged; // 注册事件通知CurrentDesignModel修改属性
        CurrentDesignModel.SetSectionEffectiveWidth();
    }

    private void OnSectionPropertyChanged(object sender, EventArgs e) {
        //MessageBox.Show("ChineseCode中的截面发生改变");
        CurrentDesignModel.UpdateGeneralProperties();
        CurrentDesignModel.UpdateDesignProperties();
    }
}