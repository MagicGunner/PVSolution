using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.UIModels;

public partial class PostModel : ObservableObject {
    [ObservableProperty]
    private bool _isSelected; // 当前立柱是否被选中

    [ObservableProperty]
    private int _num; // 立柱序号

    [ObservableProperty]
    private bool _isDrive; // 是否驱动

    [ObservableProperty]
    private bool _isMotor; // 是否回转

    [ObservableProperty]
    private string? _sectionType; // 立柱截面类型

    [ObservableProperty]
    private string? _section; // 立柱截面

    [ObservableProperty]
    private string? _material; // 立柱材质

    [ObservableProperty]
    private double _pileUpGround; // 基础露头

    [ObservableProperty]
    private double _pileDownGround; // 基础埋深

    [ObservableProperty]
    private double _leftToBeam; // 立柱左侧开断

    [ObservableProperty]
    private double _rightToBeam; // 立柱右侧开断

    [ObservableProperty]
    private double _pileWidth; // 基础宽度

    [ObservableProperty]
    private double _leftSpan; // 左侧跨距

    [ObservableProperty]
    private double _rightSpan; // 右侧跨距

    [ObservableProperty]
    private List<string> _sectionTypeList = TemplateData.Current.PostSectionMap.Keys.ToList();


    public List<string>? SectionList {
        get {
            if (SectionType == null) {
                return null;
            } else {
                return TemplateData.Current.PostSectionMap[SectionType].Select(item => item.Name).ToList();
            }
        }
    }

    [ObservableProperty]
    private bool isDetailsVisible;
}