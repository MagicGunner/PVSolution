﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using CADToolBox.Shared.Models.CADModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.CADModels.Implement;

/// <summary>
/// 所有坐标的起点都是以组件的最左端
/// </summary>
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
        ModuleColCounter * ModuleWidth                              + (ModuleColCounter - 1) * ModuleGapAxis +
        (DriveGap == 0 ? 0 : DriveNum * (DriveGap - ModuleGapAxis)) + LeftRemind + RightRemind;

    // 弦长
    public double Chord => ModuleRowCounter * ModuleLength + (ModuleRowCounter - 1) * ModuleGapChord;

    // 旋转中心离地距离
    public double BeamCenterToGround =>
        MinGroundDist + 0.5 * Chord * Math.Sin(Math.PI * MaxAngle        / 180) -
        (PurlinHeight + BeamHeight * (1 + BeamRadio)) * Math.Cos(Math.PI * MaxAngle / 180);

    #endregion

    #region 计算干涉需要用到的方法，当立柱或主梁数量长度发生改变时需要通知

    // 当立柱数量发生变化或者立柱类型发生变化时触发
    public void OnPostListChanged() {
        OnPropertyChanged(nameof(DriveNum));
        OnPropertyChanged(nameof(SystemLength));

        SortPost();
        SortBeam();
    }

    #endregion

    #region 构造函数

    public TrackerModel() {
        ProjectName = "跟踪支架GA图";
    }

    #endregion

    #region 绘图辅助,只更新坐标，属性在前台更新

    // 更新立柱中心线坐标
    // !!!!!!!!!!!!有驱动间隙时需要考虑檩条和主梁的坐标变化
    public void SortPost() {
        if (PostList == null) return;
        PostList[0].X = PostList[0].LeftSpan;
        for (var i = 1; i < PostList.Count; i++) {
            PostList[i].X = PostList[i - 1].X + PostList[i].LeftSpan;
        }
    }

    // 更新主梁两端点坐标
    // !!!!!!!!!增加主梁间隙与立柱处的开断
    public void SortBeam() {
        if (BeamList == null) return;

        BeamList[0].StartX = -LeftRemind;
        BeamList[0].EndX   = -LeftRemind + BeamList[0].Length;
        for (var i = 1; i < BeamList.Count; i++) {
            BeamList[i].StartX = BeamList[i - 1].EndX + BeamList[i].LeftToPre;
            BeamList[i].EndX   = BeamList[i].StartX   + BeamList[i].Length;
        }
    }

    // 更新檩条坐标
    public void InitPurlin() {
        PurlinList = new List<PurlinModel> { Capacity = 0 };
        var x = -ModuleGapAxis / 2;
        PurlinList.Add(new PurlinModel(x, -1)); // 最左侧檩条
        for (var i = 0; i < ModuleColCounter - 1; i++) {
            x += ModuleWidth + ModuleGapAxis;
            PurlinList.Add(new PurlinModel(x, 0));
        }

        x += ModuleWidth + ModuleGapAxis;

        PurlinList.Add(new PurlinModel(x, 1)); // 最右侧檩条
    }

    // 当立柱发生改变时需要触发，执行时默认立柱跨距已经填好，执行完成可能更新立柱的位置
    public void UpdatePurlin() {
        InitPurlin();
        if (DriveGap <= 0) return;
        if (PostList == null) return;

        var drivePostList = PostList.Where(post => post.IsDrive);
        foreach (var drivePost in drivePostList) {
            for (var i = 0; i < PurlinList?.Count; i++) {
                if (Math.Abs(PurlinList[i].X - drivePost.X) <= (ModuleWidth + ModuleGapAxis) / 2) {
                    var adjustDist = PurlinList[i].X + (DriveGap - ModuleGapAxis) / 2 - drivePost.X; // 立柱需要移动的距离,向右为正
                    drivePost.X         += adjustDist;
                    drivePost.LeftSpan  += adjustDist;
                    drivePost.RightSpan -= adjustDist;
                    // 左侧檩条往左移动
                    for (var j = 0; j <= i; j++) {
                        PurlinList[j].X -= (DriveGap - ModuleGapAxis) / 2;
                    }

                    // 右侧檩条往右移动
                    for (var j = i + 1; j < PurlinList.Count; j++) {
                        PurlinList[j].X += (DriveGap - ModuleGapAxis) / 2;
                    }

                    PurlinList[i].Type = -1;                                                                    // 左檩条
                    PurlinList.Insert(i + 1, new PurlinModel(PurlinList[i].X + (DriveGap - ModuleGapAxis), 1)); // 右檩条
                }
            }
        }
    }

    // 初始化,更新所有坐标满足实际情况
    public void Init() {
        InitPurlin();


        if (BeamList?.Count > 0) {
            foreach (var beamModel in BeamList) {
                beamModel.LeftToPre   = BeamGap;
                beamModel.RightToNext = BeamGap;
            }

            SortBeam();

            // 主梁初始化时需要保证主梁长度没问题
            while (BeamList.Last().EndX > SystemLength) {
                if (BeamList.Last().StartX < SystemLength) {
                    BeamList.Last().EndX   = SystemLength;
                    BeamList.Last().Length = SystemLength - BeamList.Last().StartX;
                } else {
                    BeamList.RemoveAt(BeamList.Count - 1);
                }
            }
        }

        if (!(PostList?.Count > 0)) return;
        SortPost();
        if (DriveGap <= 0) return;

        var drivePostList = PostList.Where(post => post.IsDrive).ToList();
        for (var i = 0; i < drivePostList.Count; i++) {
            var drivePost = drivePostList[i];
            // 计算当前驱动立柱前方可以放多少组件
            var drivePostToFirstPurlin = drivePost.X + ModuleGapAxis / 2 - (DriveGap - ModuleGapAxis) / 2;
            drivePostToFirstPurlin -= i * (DriveGap                      - ModuleGapAxis);
            var purlinNum    = drivePostToFirstPurlin / (ModuleWidth + ModuleGapAxis);
            var modifyFactor = purlinNum - (int)purlinNum;
            if (modifyFactor <= 0.5) { // 靠前一个檩条近
                drivePost.X         -= modifyFactor * (ModuleWidth + ModuleGapAxis);
                drivePost.LeftSpan  -= modifyFactor * (ModuleWidth + ModuleGapAxis);
                drivePost.RightSpan += modifyFactor * (ModuleWidth + ModuleGapAxis);
            } else {
                drivePost.X         += (1 - modifyFactor) * (ModuleWidth + ModuleGapAxis);
                drivePost.LeftSpan  += (1 - modifyFactor) * (ModuleWidth + ModuleGapAxis);
                drivePost.RightSpan -= (1 - modifyFactor) * (ModuleWidth + ModuleGapAxis);
            }
        }
    }

    #endregion
}