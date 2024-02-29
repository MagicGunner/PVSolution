using System.Collections.Generic;
using System.Linq;
using CADToolBox.Resource.NameDictionarys;
using CADToolBox.Shared.Models.CADModels.Implement;
using CADToolBox.Shared.Models.UIModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.UIModels.Implement;

public partial class BeamInfo(
    BeamModel beamModel
) : ObservableObject, ITrackerItemInfo {
#region 自身属性

    public BeamModel BeamModel => beamModel;

    [ObservableProperty]
    private bool _isDetailsVisible; // 细节是否展示

    [ObservableProperty]
    private bool _isSelected; // 当前主梁是否被选中

    [ObservableProperty]
    private List<string> _sectionTypeList = GeneralTemplateData.BeamSectionMap.Keys.ToList();

    public List<string>? SectionList {
        get {
            return SectionType == null
                       ? null
                       : GeneralTemplateData.BeamSectionMap[SectionType].Select(item => item.Name).ToList();
        }
    }

#endregion

#region 主梁模型属性

    public int Num {
        get => BeamModel.Num;
        set =>
            SetProperty(BeamModel.Num, value, BeamModel, (model,
                                                          value) => model.Num = value);
    }

    public string? SectionType {
        get => BeamModel.SectionType;
        set =>
            SetProperty(BeamModel.SectionType, value, BeamModel, (model,
                                                                  value) => model.SectionType = value);
    }

    public string? Section {
        get => BeamModel.Section;
        set =>
            SetProperty(BeamModel.Section, value, BeamModel, (model,
                                                              value) => model.Section = value);
    }

    public string? Material {
        get => BeamModel.Material;
        set =>
            SetProperty(BeamModel.Material, value, BeamModel, (model,
                                                               value) => model.Material = value);
    }

    public double LeftToPre {
        get => BeamModel.LeftToPre;
        set =>
            SetProperty(BeamModel.LeftToPre, value, BeamModel, (model,
                                                                value) => model.LeftToPre = value);
    }

    public double RightToNext {
        get => BeamModel.RightToNext;
        set =>
            SetProperty(BeamModel.RightToNext, value, BeamModel, (model,
                                                                  value) => model.RightToNext = value);
    }

    public double Length {
        get => BeamModel.Length;
        set =>
            SetProperty(BeamModel.Length, value, BeamModel, (model,
                                                             value) => model.Length = value);
    }

#endregion
}