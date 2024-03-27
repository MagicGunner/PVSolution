using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Prism.Ioc;
using Prism.Mvvm;
using SapToolBox.Modules.CommonTools.Models.UIModels;
using SapToolBox.Resource.DesignResources;

namespace SapToolBox.Modules.CommonTools.ViewModels.SubViewModels;

public class SectionDefViewModel : BindableBase {
    public double CanvasWidth { // 前台画布宽度
        get => CurrentSection.CanvasWidth;
        set => SetProperty(ref CurrentSection.CanvasWidth, value);
    }

    public double CanvasHeight { // 前台画布宽度
        get => CurrentSection.CanvasHeight;
        set => SetProperty(ref CurrentSection.CanvasHeight, value);
    }


    private readonly IContainerProvider _containerProvider;

    private ObservableCollection<SectionInfo>? _sectionList;

    public ObservableCollection<SectionInfo>? SectionList {
        get => _sectionList;
        set => SetProperty(ref _sectionList, value);
    }

    private SectionInfo _currentSection = new();

    public SectionInfo CurrentSection {
        get => _currentSection;
        set => SetProperty(ref _currentSection, value);
    }

    public List<string>? SectionTypeList { get; set; }

    private ObservableCollection<string>? _sectionNameList;

    public ObservableCollection<string>? SectionNameList {
        get => _sectionNameList;
        set => SetProperty(ref _sectionNameList, value);
    }

    public SectionDefViewModel(IContainerProvider containerProvider) {
        _containerProvider = containerProvider;

        SectionTypeList = GeneralTemplateData.PostSectionMap.Keys.ToList(); // 导入有预设截面规格的截面类型
        SectionTypeList.Add("焊接H型钢");
        SectionTypeList.Add("折弯C型钢");
        SectionTypeList.Add("折弯角钢");
        SectionTypeList.Add("折弯几字型钢");
        SectionTypeList.Add("方管");
        SectionTypeList.Add("矩形管");
        CurrentSection.PropertyChanged += OnCurrentSectionChanged;
    }

    private void OnCurrentSectionChanged(object                   obj,
                                         PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(CurrentSection.SectionType): // 当截面类型发生改变事需要更新SectionNameList
                if (CurrentSection.SectionType != null) {
                    if (GeneralTemplateData.PostSectionMap.TryGetValue(CurrentSection.SectionType, out var value)) {
                        // 如果截面库中有该种截面类型
                        SectionNameList = new ObservableCollection<string>(value.Select(item => item.Name).ToList());
                        CurrentSection.IsEditable = false;
                    } else {
                        SectionNameList           = null;
                        CurrentSection.IsEditable = true;
                    }
                }

                break;
        }
    }
}