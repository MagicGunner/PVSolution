using CADToolBox.Shared.Models.CADModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.CADModels.Implement.Fixed;

public partial class FixedModel : ObservableObject, IPvSupport {
    #region 通用属性

    [ObservableProperty]
    private string? _projectName;

    [ObservableProperty]
    private double _moduleLength;

    [ObservableProperty]
    private double _moduleWidth;

    [ObservableProperty]
    private double _moduleHeight;

    [ObservableProperty]
    private double _moduleGapChord;

    [ObservableProperty]
    private double _moduleGapAxis;

    [ObservableProperty]
    private double _minGroundDist;

    [ObservableProperty]
    private double _pileUpGround;

    [ObservableProperty]
    private double _pileDownGround;

    [ObservableProperty]
    private double _pileWidth;

    [ObservableProperty]
    private int _moduleRowCounter;

    [ObservableProperty]
    private int _moduleColCounter;

    #endregion

    #region 固定支架特有属性

    [ObservableProperty]
    private double _title; // 摆放角度

    #endregion
}