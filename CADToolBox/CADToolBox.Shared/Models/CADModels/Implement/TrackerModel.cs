using System;
using System.Collections.Generic;
using System.Windows.Documents;
using CADToolBox.Shared.Models.CADModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.CADModels.Implement;

public partial class TrackerModel : ObservableObject, IPvSupport {
#region 通用属性

    [ObservableProperty]
    private string? _projectName;

    [ObservableProperty]
    private double _moduleLength;

    [ObservableProperty]
    private double _moduleHeight;

    [ObservableProperty]
    private double _moduleWidth;

    [ObservableProperty]
    private double _moduleGapChord;

    [ObservableProperty]
    private double _moduleGapAxis;

    [ObservableProperty]
    private double _minGroundDist;

    [ObservableProperty]
    private double _pileUpGround;

    [ObservableProperty]
    private double _pileWidth;

#endregion

#region 跟踪支架属性

    [ObservableProperty]
    private double _purlinHeight; // 檩条高度

    [ObservableProperty]
    private double _purlinWidth; // 檩条宽度

    [ObservableProperty]
    private double _purlinLength; // 檩条长度

    [ObservableProperty]
    private double _beamHeight; // 主梁高度

    [ObservableProperty]
    private double _beamWidth; // 主梁宽度

    [ObservableProperty]
    private int _moduleRowCounter; // 组件排数

    [ObservableProperty]
    private int _moduleColCounter; // 组件列数

    [ObservableProperty]
    private double _stowAngle; // 保护角度

    [ObservableProperty]
    private double _maxAngle; // 最大角度

    [ObservableProperty]
    private double _beamCenterToDrivePost; // 旋转中心到驱动立柱顶部距离

    [ObservableProperty]
    private double _beamCenterToGeneralPost; // 旋转中性到普通立柱顶部距离

    [ObservableProperty]
    private double _beamRadio; // 主梁上下部分比值

    [ObservableProperty]
    private double _postWidth; // 立柱宽度

    [ObservableProperty]
    private double _driveGap; // 驱动间隙

    [ObservableProperty]
    private double _beamGap; // 主梁间隙

    [ObservableProperty]
    private double _leftRemind; // 左侧末端余量

    [ObservableProperty]
    private double _rightRemind; // 右侧末端余量

#endregion

#region 对象属性

    public List<PostModel>? PostList { get; set; }
    public List<BeamModel>? BeamList { get; set; }

#endregion

#region 构造函数

    //public TrackerModel(double moduleLength) {
    //    ModuleLength = moduleLength;
    //}

#endregion
}