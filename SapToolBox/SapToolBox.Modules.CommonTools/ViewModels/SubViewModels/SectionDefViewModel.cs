using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using SAP2000v1;
using SapToolBox.Modules.CommonTools.Models.UIModels;
using SapToolBox.Resource.DesignResources;
using SapToolBox.Shared.Helpers;
using SapToolBox.Shared.Models.UIModels.Implement;
using SapToolBox.Shared.Prism;
using static ImTools.ImMap;
using IMapper = AutoMapper.IMapper;
using MapperConfiguration = AutoMapper.MapperConfiguration;

namespace SapToolBox.Modules.CommonTools.ViewModels.SubViewModels;

public class SectionDefViewModel : BindableBase {
#region 字段属性

    public SapModelHelper ModelHelper { get; set; }

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

    private ObservableCollection<SectionInfo>? _selectedSectionList;

    public ObservableCollection<SectionInfo>? SelectedSectionList {
        get => _selectedSectionList;
        set => SetProperty(ref _selectedSectionList, value);
    }

    private SectionInfo _currentSection;

    public SectionInfo CurrentSection {
        get => _currentSection;
        set => SetProperty(ref _currentSection, value);
    }

    public List<string>? SectionTypeList { get; set; }

    public Dictionary<string, bool> SectionDic { get; set; } = new();

    private ObservableCollection<string>? _sectionNameList;

    public ObservableCollection<string>? SectionNameList {
        get => _sectionNameList;
        set => SetProperty(ref _sectionNameList, value);
    }

    // 深拷贝工具
    private readonly MapperConfiguration _sectionConfiguration = new(cfg => cfg.CreateMap<SectionInfo, SectionInfo>());
    private          IMapper             SectionInfoMapper => _sectionConfiguration.CreateMapper();

#endregion


    public SectionDefViewModel(IContainerProvider containerProvider) {
        _containerProvider = containerProvider;
        ModelHelper = containerProvider.Resolve<SapModelHelper>();
        _currentSection = new SectionInfo {
                                              IsClose = false,
                                              IsEditable = false,
                                              SectionType = "W型钢",
                                              SectionName = "W8x8",
                                              Material = "Q355"
                                          };
        SectionNameList = new ObservableCollection<string>(GeneralTemplateData.PostSectionMap[_currentSection.SectionType].Select(item => item.Name).ToList());
        SectionList = [];
        SelectedSectionList = [];
        SectionSelectionChangedCommand = new DelegateCommand<IList>(SectionSelectionChanged);
        AddSectionCommand = new DelegateCommand(AddSection);
        DeleteSectionCommand = new DelegateCommand(DeleteSection);
        EditSectionCommand = new DelegateCommand(EditSection);
        ExportToSap2000Command = new DelegateCommand(ExportToSap2000);
        // 需要剔除的截面类型列表
        var excludeSectionTypes = new List<string>();
        excludeSectionTypes.Add("无缝钢管");
        excludeSectionTypes.Add("热轧槽钢");
        excludeSectionTypes.Add("宽翼缘H型钢(HW)");
        excludeSectionTypes.Add("中翼缘H型钢(HM)");
        excludeSectionTypes.Add("窄翼缘H型钢(HN)");
        excludeSectionTypes.Add("窄翼缘H型钢(HN)");
        excludeSectionTypes.Add("薄壁H型钢(HT)");

        SectionTypeList = GeneralTemplateData.PostSectionMap.Keys.Where(key => !excludeSectionTypes.Contains(key)).ToList(); // 导入有预设截面规格的截面类型
        SectionTypeList.Add("焊接H型钢");
        SectionTypeList.Add("折弯C型钢");
        SectionTypeList.Add("折弯槽钢");
        SectionTypeList.Add("折弯角钢");
        SectionTypeList.Add("折弯几字型钢");
        SectionTypeList.Add("方管");
        SectionTypeList.Add("矩形管");
        CurrentSection.PropertyChanged += OnCurrentSectionChanged;
    }

#region 委托声明

    // 截面选中改变
    public DelegateCommand<IList> SectionSelectionChangedCommand { get; private set; }

    // 将当前截面添加至列表
    public DelegateCommand AddSectionCommand      { get; private set; }
    public DelegateCommand DeleteSectionCommand   { get; private set; }
    public DelegateCommand EditSectionCommand     { get; private set; }
    public DelegateCommand ExportToSap2000Command { get; private set; }

#endregion

#region 委托实现

    private void SectionSelectionChanged(IList selectedItems) {
        SelectedSectionList = [];
        foreach (var selectedItem in selectedItems) {
            SelectedSectionList.Add((SectionInfo)selectedItem);
        }
    }


    private void AddSection() {
        if (CurrentSection.SectionName == null) {
            MessageBox.Show("请选择正确的截面");
            return;
        }

        if (CurrentSection.Material == null) {
            MessageBox.Show("请选择截面材质");
            return;
        }

        CurrentSection.DisplayName = CurrentSection.SectionName + "-" + CurrentSection.Material;
        if (SectionDic.ContainsKey(CurrentSection.DisplayName)) {
            MessageBox.Show("不可添加重复截面");
        } else {
            SectionDic.Add(CurrentSection.DisplayName, true);
            SectionList!.Add(SectionInfoMapper.Map<SectionInfo, SectionInfo>(CurrentSection));
        }
    }

    private void DeleteSection() {
        if (SelectedSectionList == null || SectionList == null) return;


        foreach (var sectionInfo in SelectedSectionList) {
            SectionList.Remove(sectionInfo);
        }
    }

    private void EditSection() {
        if (SelectedSectionList!.Count == 0) return; // 当没有截面选中时直接退出

        var needEditSection = SelectedSectionList.First();
        CurrentSection = needEditSection;
        SectionNameList = new ObservableCollection<string>(GeneralTemplateData.PostSectionMap[needEditSection.SectionType!].Select(item => item.Name).ToList());
        SectionList!.Remove(needEditSection);
    }

    private void ExportToSap2000() {
        cHelper myHelper = new Helper();
        var sapObject = myHelper.GetObject("CSI.SAP2000.API.SapObject");
        var sapModel = sapObject?.SapModel;
        if (sapModel == null) {
            MessageBox.Show("当前没有打开的Sap2000程序");
            return;
        }

        ModelHelper.SapModel = sapModel;
        if (SectionList == null) return;


        foreach (var sectionInfo in SectionList) {
            sectionInfo.InitISection();
            ModelHelper.AddSection(sectionInfo.Section!, eMatType.Steel);
        }
    }

#endregion

#region 方法事件区

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
                        SectionNameList = null;
                        CurrentSection.IsEditable = true;
                    }
                }

                break;
        }
    }

#endregion
}