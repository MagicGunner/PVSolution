using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using AutoMapper;
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

    private readonly MapperConfiguration _postModelConfig = new(cfg => cfg.CreateMap<PostModel, PostModel>());
    private readonly MapperConfiguration _beamModelConfig = new(cfg => cfg.CreateMap<BeamModel, BeamModel>());

    private IMapper PostMapper => _postModelConfig.CreateMapper();
    private IMapper BeamMapper => _beamModelConfig.CreateMapper();

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

    #region 构造函数

    public TrackerModel() {
        ProjectName = "跟踪支架GA图";
    }

    #endregion

    #region 绘图辅助,只更新坐标，属性在前台更新

    // 更新立柱中心线坐标,同时更新檩条
    public void InitPost() {
        if (PostList == null) return;
        PostList[0].X = PostList[0].LeftSpan;
        for (var i = 1; i < PostList.Count; i++) {
            PostList[i].X = PostList[i - 1].X + PostList[i].LeftSpan;
        }

        // 调整驱动立柱的位置,同时调整檩条的位置
        if (DriveGap > 0 && PostList != null) { // 如果驱动间隙大于0需要调整驱动立柱的位置，否则不需要
            var drivePostList = PostList.Where(post => post.IsDrive).ToList();
            // 调整立柱
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

                if (drivePost.Num > 1) {
                    PostList[drivePost.Num - 2].RightSpan = drivePost.LeftSpan;
                }

                if (drivePost.Num < PostList.Count) {
                    PostList[drivePost.Num].LeftSpan = drivePost.RightSpan;
                }
            }

            while (Math.Abs(PostList.Last().X + PostList.Last().RightSpan - (SystemLength - LeftRemind - RightRemind)) >
                   0.0001) {
                PostList.Last().RightSpan = SystemLength - LeftRemind - RightRemind - PostList.Last().X;
            }

            InitPurlinWithDriveGap();
        } else { // 没有驱动间隙檩条直接平铺
            InitPurlinWithOutDriveGap();
        }
    }

    // 初始化主梁列表，记录主梁两侧的Model
    public void InitBeam() {
        if (BeamList == null) return;

        BeamList[0].StartX = -LeftRemind;
        BeamList[0].EndX   = -LeftRemind + BeamList[0].Length;
        for (var i = 1; i < BeamList.Count; i++) {
            BeamList[i].StartX       = BeamList[i - 1].EndX + BeamList[i].LeftToPre;
            BeamList[i].EndX         = BeamList[i].StartX   + BeamList[i].Length;
            BeamList[i].PreItem      = BeamList[i - 1];
            BeamList[i - 1].NextItem = BeamList[i];
            if (i != BeamList.Count - 1) {
                BeamList[i].NextItem = BeamList[i + 1];
            }
        }

        // 调整驱动立柱的位置,同时调整檩条的位置
        if (!(DriveGap > 0) || PostList == null) return; // 如果驱动间隙大于0需要调整驱动立柱的位置，否则不需要
        // 更新主梁开断
        foreach (var drivePost in PostList.Where(post => post.IsDrive)) {
            if (drivePost.X + drivePost.RightToBeam < BeamList.First().StartX) continue; // 立柱驱动立柱在主梁最左侧的左边不处理
            for (var j = 0; j < BeamList.Count; j++) {
                if (drivePost.X + drivePost.RightToBeam > BeamList[j].StartX &&
                    drivePost.X - drivePost.LeftToBeam  <= BeamList[j].StartX) { // 第一种情况主梁左侧接上，无需增减主梁
                    BeamList[j].StartX    = drivePost.X          + drivePost.RightToBeam;
                    BeamList[j].Length    = BeamList[j].EndX     - BeamList[j].StartX;
                    BeamList[j].LeftToPre = drivePost.LeftToBeam + drivePost.RightToBeam;
                    BeamList[j].PreItem   = drivePost;
                } else if (drivePost.X - drivePost.LeftToBeam  > BeamList[j].StartX && // 驱动立柱整体在主梁中间，需要将主梁分成两段
                           drivePost.X + drivePost.RightToBeam <= BeamList[j].EndX) {
                    var newBeam = BeamMapper.Map<BeamModel, BeamModel>(BeamList[j]);
                    // 原来位置的主梁向后移动
                    BeamList[j].StartX    = drivePost.X          + drivePost.RightToBeam;
                    BeamList[j].Length    = BeamList[j].EndX     - BeamList[j].StartX;
                    BeamList[j].LeftToPre = drivePost.LeftToBeam + drivePost.RightToBeam;
                    BeamList[j].PreItem   = drivePost;
                    newBeam.EndX          = drivePost.X          - drivePost.LeftToBeam;
                    newBeam.Length        = newBeam.EndX         - newBeam.StartX;
                    newBeam.RightToNext   = drivePost.LeftToBeam + drivePost.RightToBeam;
                    newBeam.NextItem      = drivePost;
                    BeamList.Insert(j, newBeam);
                } else if (drivePost.X + drivePost.RightToBeam > BeamList[j].EndX && // 主梁右侧接上
                           drivePost.X - drivePost.LeftToBeam  <= BeamList[j].EndX) {
                    BeamList[j].EndX        = drivePost.X          - drivePost.LeftToBeam;
                    BeamList[j].Length      = BeamList[j].EndX     - BeamList[j].StartX;
                    BeamList[j].RightToNext = drivePost.LeftToBeam + drivePost.RightToBeam;
                    BeamList[j].NextItem    = drivePost;
                }
            }
        }

        // 对主梁操作后最后一段主梁长度可能有变化
        while (Math.Abs(BeamList.Last().EndX - (SystemLength - LeftRemind)) > 0.0001) {
            BeamList.Last().EndX   = SystemLength         - LeftRemind;
            BeamList.Last().Length = BeamList.Last().EndX - BeamList.Last().StartX;
        }
    }

    // 更新主梁列表
    public void UpdateBeam() {
        if (BeamList == null) {
            return;
        }

        var startX = -LeftRemind;
        for (var i = 0; i < BeamList.Count; i++) {
            if (startX >= SystemLength - LeftRemind) {
                BeamList[i].StartX = SystemLength - LeftRemind;
                BeamList[i].EndX   = SystemLength - LeftRemind;
                BeamList[i].Length = 0;
            } else {
                BeamList[i].StartX = startX;
                BeamList[i].EndX   = Math.Min(BeamList[i].StartX + BeamList[i].Length, SystemLength - LeftRemind);
                BeamList[i].Length = BeamList[i].EndX - BeamList[i].StartX;
                // 更新到前一个主梁的距离
                if (i == 0) {
                    BeamList[i].LeftToPre = 0;
                } else {
                    BeamList[i].LeftToPre = BeamList[i - 1].RightToNext;
                }

                BeamList[i].RightToNext = BeamList[i].NextItem switch {
                                              // 更新到后一个主梁的距离
                                              PostModel drivePost => drivePost.LeftToBeam +
                                                                     drivePost.RightToBeam, // 下一个为驱动立柱代表间隙为立柱处的开断
                                              BeamModel => BeamGap,                         // 下一个为主梁填入主梁间隙
                                              _         => 0,                               // 下个为空时代表最后一个主梁 
                                          };

                startX += BeamList[i].EndX + BeamList[i].RightToNext;
            }
        }
    }

    // 更新檩条坐标
    public void InitPurlinWithOutDriveGap() {
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
    public void InitPurlinWithDriveGap() {
        PurlinList = new List<PurlinModel> { Capacity = 0 };
        var    drivePosts = PostList!.Where(post => post.IsDrive);
        var    startX     = -ModuleGapAxis / 2;
        double endX;
        foreach (var drivePost in drivePosts) {
            PurlinList.Add(new PurlinModel(startX, -1));
            endX = drivePost.X - (DriveGap - ModuleGapAxis) / 2;
            do {
                startX += ModuleWidth + ModuleGapAxis;
                PurlinList.Add(new PurlinModel(startX, 0));
            } while (startX < endX);

            PurlinList.Last().Type = 1;
            startX                 = drivePost.X + (DriveGap - ModuleGapAxis) / 2;
        }

        PurlinList.Add(new PurlinModel(startX, -1));
        endX = SystemLength - LeftRemind - RightRemind + ModuleGapAxis / 2;
        do {
            startX += ModuleWidth + ModuleGapAxis;
            PurlinList.Add(new PurlinModel(startX, 0));
        } while (startX < endX);

        PurlinList.Last().Type = 1;
    }

    public void InitBeamGap() {
        if (BeamList == null) return;
        foreach (var beamModel in BeamList) {
            beamModel.LeftToPre   = BeamGap;
            beamModel.RightToNext = BeamGap;
        }
    }

    public void InitDriveSplit() {
        if (PostList == null) {
            return;
        }

        var drivePostList = PostList.Where(post => post.IsDrive);
        foreach (var drivePost in drivePostList) {
            drivePost.LeftToBeam  = 75;
            drivePost.RightToBeam = 75;
        }
    }

    public void Init() {
        InitDriveSplit();
        InitBeamGap();

        // 删除整理多余的主梁
        if (BeamList != null) {
            var totalLength = SystemLength;
            BeamList[0].Length =  Math.Min(totalLength, BeamList[0].Length);
            totalLength        -= BeamList[0].Length;
            for (var i = 1; i < BeamList.Count; i++) {
                if (totalLength <= 0) {
                    BeamList[i].Length = 0;
                } else {
                    BeamList[i].Length =  Math.Min(BeamList[i].Length, totalLength);
                    totalLength        -= BeamList[i].Length + BeamList[i].LeftToPre;
                }
            }

            while (BeamList.Last().Length == 0) {
                BeamList.RemoveAt(BeamList.Count - 1);
            }
        }

        // 整理删除多余的立柱
        if (PostList != null) {
            var totalLength = SystemLength - LeftRemind - RightRemind;
            PostList[0].X = PostList[0].LeftSpan;
            for (var i = 1; i < PostList.Count; i++) {
                PostList[i].LeftSpan = PostList[i - 1].RightSpan;
                PostList[i].X        = PostList[i - 1].X + PostList[i].LeftSpan;
            }

            while (PostList.Last().X > totalLength) {
                PostList.RemoveAt(PostList.Count - 1);
            }

            PostList.Last().RightSpan = totalLength - PostList.Last().X;
        }


        InitPost(); // 根据立柱与檩条
        // 初始化时将主梁的间隙保存到对象
        InitBeam(); // 初始化主梁
    }

    #endregion
}