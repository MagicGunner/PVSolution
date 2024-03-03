using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
    [NotifyPropertyChangedFor(nameof(Chord))]
    [NotifyPropertyChangedFor(nameof(BeamCenterToGround))]
    private double _moduleLength;

    [ObservableProperty]
    private double _moduleHeight;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SystemLength))]
    private double _moduleWidth;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Chord))]
    [NotifyPropertyChangedFor(nameof(BeamCenterToGround))]
    private double _moduleGapChord;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SystemLength))]
    private double _moduleGapAxis;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BeamCenterToGround))]
    private double _minGroundDist;

    [ObservableProperty]
    private double _pileUpGround;

    [ObservableProperty]
    private double _pileWidth;

#endregion

#region 跟踪支架属性

    [ObservableProperty]
    private bool _hasSlew;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BeamCenterToGround))]
    private double _purlinHeight; // 檩条高度

    [ObservableProperty]
    private double _purlinWidth; // 檩条宽度

    [ObservableProperty]
    private double _purlinLength; // 檩条长度

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BeamCenterToGround))]
    private double _beamHeight; // 主梁高度

    [ObservableProperty]
    private double _beamWidth; // 主梁宽度

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Chord))]
    [NotifyPropertyChangedFor(nameof(BeamCenterToGround))]
    private int _moduleRowCounter; // 组件排数

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SystemLength))]
    private int _moduleColCounter; // 组件列数

    [ObservableProperty]
    private double _stowAngle; // 保护角度

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BeamCenterToGround))]
    private double _maxAngle; // 最大角度

    [ObservableProperty]
    private double _beamCenterToDrivePost; // 旋转中心到驱动立柱顶部距离

    [ObservableProperty]
    private double _beamCenterToGeneralPost; // 旋转中性到普通立柱顶部距离

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BeamCenterToGround))]
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

#endregion

#region 对象属性与计算属性

    public List<PostModel>? PostList { get; set; }
    public List<BeamModel>? BeamList { get; set; }

    public List<PurlinModel>? PurlinList { get; set; }

    // 驱动立柱数目
    public int DriveNum => PostList?.Count(post => post.IsDrive) ?? 0;

    // 系统总长
    public double SystemLength =>
        ModuleColCounter       * ModuleWidth
      + (ModuleColCounter - 1) * ModuleGapAxis
      + (DriveGap == 0 ? 0 : DriveNum * (DriveGap - ModuleGapAxis))
      + LeftRemind
      + RightRemind;

    // 弦长
    public double Chord => ModuleRowCounter * ModuleLength + (ModuleRowCounter - 1) * ModuleGapChord;

    // 旋转中心离地距离
    public double BeamCenterToGround =>
        MinGroundDist
      + 0.5 * Chord * Math.Sin(Math.PI * MaxAngle / 180)
      - (PurlinHeight + BeamHeight * (1 + BeamRadio)) * Math.Cos(Math.PI * MaxAngle / 180);

#endregion

#region 计算干涉需要用到的方法，当立柱或主梁数量长度发生改变时需要通知

    // 当立柱数量发生变化或者立柱类型发生变化时触发
    public void OnPostListChanged() {
        OnPropertyChanged(nameof(DriveNum));
        OnPropertyChanged(nameof(SystemLength));

        SortPostX();
        SortBeamX();
    }

#endregion

#region 构造函数

    public TrackerModel() {
        ProjectName = "跟踪支架GA图";
    }

#endregion

#region 绘图辅助

    // 更新立柱中心线坐标
    // !!!!!!!!!!!!有驱动间隙时需要考虑檩条的坐标变化
    public void SortPostX() {
        if (PostList == null) return;
        PostList[0].X = PostList[0].LeftSpan;
        for (var i = 1; i < PostList.Count; i++) { PostList[i].X = PostList[i - 1].X + PostList[i].LeftSpan; }
    }

    // 更新主梁两端点坐标
    // !!!!!!!!!增加主梁间隙与立柱处的开断
    public void SortBeamX() {
        if (BeamList == null) return;

        BeamList[0].StartX = -LeftRemind;
        BeamList[0].EndX   = -LeftRemind + BeamList[0].Length;
        for (var i = 1; i < BeamList.Count; i++) {
            var startX = BeamList[i - 1].EndX + BeamList[i].LeftToPre;
            if (startX >= SystemLength) { BeamList[i].Length = 0; }

            var endX = Math.Min(startX + BeamList[i].Length, SystemLength);
            BeamList[i].StartX = startX;
            BeamList[i].EndX   = endX;
            BeamList[i].Length = endX - startX;
        }
    }

    // 更新檩条坐标
    public void InitPurlin() {
        PurlinList = new List<PurlinModel> {
                                               Capacity = 0
                                           };
        var x = -ModuleGapAxis / 2;
        PurlinList.Add(new PurlinModel(x, -1)); // 最左侧檩条
        for (var i = 0; i < ModuleColCounter - 1; i++) {
            x += ModuleWidth + ModuleGapAxis;
            PurlinList.Add(new PurlinModel(x, 0));
        }

        x += ModuleWidth + ModuleGapAxis;
        PurlinList.Add(new PurlinModel(x, 1)); // 最右侧檩条
    }

    // 初始化
    // 出图前的初始化
    public void InitBeforeDraw() {
        InitPurlin();
        SortPostX();
        SortBeamX();
    }

#endregion
}