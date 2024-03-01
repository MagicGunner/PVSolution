using System;
using System.Collections.Generic;
using System.Windows.Documents;
using CADToolBox.Shared.Models.CADModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.CADModels.Implement;

public partial class TrackerModel : ObservableObject, IPvSupport {
    public int Status { get; set; } // 当前绘图状态，0代表仅保存，1代表保存并绘图

#region 通用属性

    [ObservableProperty]
    private string? _projectName;

    [ObservableProperty]
    private double _moduleLength;

    [ObservableProperty]
    private double _moduleHeight;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SystemLength))]
    private double _moduleWidth;

    [ObservableProperty]
    private double _moduleGapChord;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SystemLength))]
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
    [NotifyPropertyChangedFor(nameof(SystemLength))]
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
    [NotifyPropertyChangedFor(nameof(SystemLength))]
    private double _driveGap; // 驱动间隙

    [ObservableProperty]
    private double _beamGap; // 主梁间隙

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SystemLength))]
    private double _leftRemind; // 左侧末端余量

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SystemLength))]
    private double _rightRemind; // 右侧末端余量

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SystemLength))]
    private double _driveNum; // 驱动立柱的数量

#endregion

#region 对象属性

    public List<PostModel>? PostList { get; set; }
    public List<BeamModel>? BeamList { get; set; }


    public double SystemLength =>
        ModuleColCounter * ModuleWidth                              + (ModuleColCounter - 1) * ModuleGapAxis +
        (DriveGap == 0 ? 0 : DriveNum * (DriveGap - ModuleGapAxis)) + LeftRemind + RightRemind;

#endregion

#region 计算干涉需要用到的方法，当立柱或主梁数量长度发生改变时需要通知

    // 当立柱数组发生改变时
    private void OnPostListChanged() {
        OnPropertyChanged(nameof(DriveNum));
        OnPropertyChanged(nameof(DriveGap));
    }

    // 当主梁数组发生改变时
    private void OnBeamListChanged() {
    }

#endregion

#region 构造函数

    public TrackerModel() {
        ProjectName = "跟踪支架GA图";
    }

#endregion

#region 绘图辅助

    public void SortPostX() {
        if (PostList == null) return;
        PostList[0].X = PostList[0].LeftSpan;
        for (var i = 1; i < PostList.Count; i++) {
            PostList[i].X = PostList[i - 1].X;
        }
    }

    public void SortBeamX() {
        if (BeamList == null) return;
        BeamList[0].StartX = -LeftRemind;
        BeamList[0].EndX   = -RightRemind + BeamList[0].Length;
        for (var i = 0; i < BeamList.Count; i++) {
            BeamList[i].StartX = BeamList[i - 1].EndX + BeamList[i].LeftToPre;
            BeamList[i].EndX   = BeamList[i].StartX   + BeamList[i].Length;
        }
    }

#endregion
}