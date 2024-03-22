﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CADToolBox.Shared.Models.CADModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.CADModels.Implement.Tracker;

/// <summary>
/// 所有坐标的起点都是以组件的最左端
/// </summary>
public partial class TrackerModel : ObservableObject, IPvSupport {
    public int Status { get; set; } // 当前绘图状态，0代表仅保存，1代表保存并绘图

    [ObservableProperty]
    private double _minBeamLength = 200;

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
    private double _pileDownGround;

    [ObservableProperty]
    private double _pileWidth;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Chord))]
    [NotifyPropertyChangedFor(nameof(BeamCenterToGround))]
    private int _moduleRowCounter; // 组件排数

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SystemLength))]
    private int _moduleColCounter; // 组件列数

    #endregion

    #region 跟踪支架属性

    [ObservableProperty]
    private string? _driveType;

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

    private double _driveGap; // 驱动间隙

    public double DriveGap {
        get => _driveGap;
        set {
            var oldValue = _driveGap;
            if (!SetProperty(ref _driveGap, value)) return;
            LeftRemind -= (value - oldValue) * DriveNum / 2;
            RightRemind -= (value - oldValue) * DriveNum / 2;
            OnPropertyChanged(nameof(SystemLength));
            OnPropertyChanged(nameof(LeftRemind));
            OnPropertyChanged(nameof(RightRemind));
        }
    }

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
    public double SystemLength => ModuleColCounter * ModuleWidth + (ModuleColCounter - 1) * ModuleGapAxis + (DriveGap == 0 ? 0 : DriveNum * (DriveGap - ModuleGapAxis)) + LeftRemind + RightRemind;

    // 弦长
    public double Chord => ModuleRowCounter * ModuleLength + (ModuleRowCounter - 1) * ModuleGapChord;

    // 旋转中心离地距离
    public double BeamCenterToGround => MinGroundDist + 0.5 * Chord * Math.Sin(Math.PI * MaxAngle / 180) - (PurlinHeight + BeamHeight * BeamRadio / (1 + BeamRadio)) * Math.Cos(Math.PI * MaxAngle / 180);

    #endregion

    #region 构造函数

    public TrackerModel() {
        ProjectName = "跟踪支架GA图";
    }

    #endregion

    #region 初始化立柱与主梁檩条坐标，当模型发生改变时更新坐标

    // 更新立柱中心线坐标，同时更新檩条与主梁
    public void InitPost() {
        if (PostList == null) return;

        // 整理删除多余的立柱,修改错误的立柱
        var totalLength = SystemLength - LeftRemind - RightRemind;
        PostList[0].X = PostList[0].LeftSpan;
        for (var i = 1; i < PostList.Count; i++) {
            PostList[i].LeftSpan = PostList[i - 1].RightSpan;
            PostList[i].X = PostList[i - 1].X + PostList[i].LeftSpan;
        }

        while (PostList.Last().X > totalLength) {
            PostList.RemoveAt(PostList.Count - 1);
        }

        PostList.Last().RightSpan = totalLength - PostList.Last().X;


        var totalSpan = SystemLength - LeftRemind - RightRemind;
        PostList[0].X = PostList[0].LeftSpan;
        PostList[0].Num = 1;
        for (var i = 1; i < PostList.Count; i++) {
            PostList[i].Num = i + 1;
            PostList[i].LeftSpan = PostList[i - 1].RightSpan;
            PostList[i].X = PostList[i - 1].X + PostList[i].LeftSpan;
            PostList[i].StartX = PostList[i].X;
            PostList[i].EndX = PostList[i].X;
        }

        // 调整驱动立柱的位置,同时调整檩条的位置
        if (DriveGap > 0 && PostList != null) { // 如果驱动间隙大于0需要调整驱动立柱的位置，否则不需要
            UpdateDrivePost();
            InitPurlinWithDriveGap();
            InitBeam();
        } else { // 没有驱动间隙檩条直接平铺
            InitPurlinWithOutDriveGap();
        }
    }

    // 初始化主梁列表，维护双向链表，在此之前需要保证立柱已经初始化完毕
    public void InitBeam() {
        if (BeamList == null) return;
        if (PostList == null) return;

        BeamList[0].Num = 1;
        BeamList[0].Length = Math.Max(Math.Min(SystemLength, BeamList[0].Length), MinBeamLength);
        BeamList[0].StartX = -LeftRemind;
        BeamList[0].EndX = -LeftRemind + BeamList[0].Length;
        for (var i = 1; i < BeamList.Count; i++) {
            BeamList[i].Num = i + 1;
            BeamList[i].StartX = BeamList[i - 1].EndX + BeamList[i].LeftToPre; // 初始化时先将所有的主梁间隙置为初始值
            BeamList[i].EndX = BeamList[i].StartX + Math.Max(BeamList[i].Length, MinBeamLength);
            if (BeamList[i].EndX > SystemLength - LeftRemind) {
                BeamList[i].EndX = SystemLength - LeftRemind;
                if (BeamList[i].StartX >= SystemLength - LeftRemind) {
                    BeamList[i].StartX = SystemLength - LeftRemind;
                }
            }

            BeamList[i].Length = BeamList[i].EndX - BeamList[i].StartX;
        }

        while (BeamList.Last().Length == 0) BeamList.RemoveAt(BeamList.Count - 1); // 移除多余的主梁

        UpdateBeamData();
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
        var drivePosts = PostList!.Where(post => post.IsDrive);
        var startX = -ModuleGapAxis / 2;
        int purlinNum;
        foreach (var drivePost in drivePosts) {
            purlinNum = Convert.ToInt32((drivePost.X - (DriveGap - ModuleGapAxis) / 2 - startX) / (ModuleWidth + ModuleGapAxis));
            PurlinList.Add(new PurlinModel(startX, -1));
            for (var i = 1; i < purlinNum + 1; i++) {
                PurlinList.Add(new PurlinModel(startX + i * (ModuleWidth + ModuleGapAxis), 0));
            }

            PurlinList.Last().Type = 1;
            startX = drivePost.X + (DriveGap - ModuleGapAxis) / 2;
        }

        purlinNum = Convert.ToInt32((SystemLength - LeftRemind - RightRemind + ModuleGapAxis - startX) / (ModuleWidth + ModuleGapAxis));
        PurlinList.Add(new PurlinModel(startX, -1));
        for (var i = 1; i < purlinNum + 1; i++) {
            PurlinList.Add(new PurlinModel(startX + i * (ModuleWidth + ModuleGapAxis), 0));
        }

        PurlinList.Last().Type = 1;
    }

    // 初始化时修正一些错误
    public void Init() {
        HasSlew = DriveType == "回转";
        if (!HasSlew) { // 推杆驱动
            DriveGap = 0;
            if (PostList != null) {
                foreach (var postModel in PostList) {
                    postModel.LeftToBeam = 0;
                    postModel.RightToBeam = 0;
                    if (postModel.IsDrive) {
                        postModel.IsMotor = false;
                    }
                }
            }
        } else { // 电机驱动
            if (PostList != null) {
                foreach (var postModel in PostList.Where(postModel => postModel.IsDrive)) {
                    postModel.IsMotor = true;
                }
            }
        }

        InitPost();
        // 初始化时将主梁的间隙保存到对象
        InitBeam(); // 初始化主梁
    }

    // 更新主梁坐标,所有前置操作在前台做，到这一步主梁除了坐标其余均正确
    public void UpdateBeamX() {
        if (BeamList!.Count == 0) return;
        UpdateBeamData(); // 保证主梁所有信息准确
        BeamList[0].Num = 1;
        BeamList[0].StartX = -LeftRemind;
        BeamList[0].EndX = BeamList[0].Length - LeftRemind;
        for (var i = 1; i < BeamList.Count; i++) {
            BeamList[i].Num = i + 1;
            switch (BeamList[i].PreItem) {
                case BeamModel beam:
                    BeamList[i].LeftToPre = BeamGap;
                    beam.RightToNext = BeamGap;
                    break;
                case PostModel drivePost:
                    BeamList[i].LeftToPre = drivePost.LeftToBeam + drivePost.RightToBeam;
                    break;
            }

            BeamList[i].StartX = BeamList[i - 1].EndX + BeamList[i].LeftToPre;
            BeamList[i].EndX = BeamList[i].StartX + BeamList[i].Length;
        }

        while (BeamList.Last().EndX > SystemLength - LeftRemind) {
            if (BeamList.Last().StartX < SystemLength - LeftRemind) {
                BeamList.Last().EndX = SystemLength - LeftRemind;
                BeamList.Last().Length = BeamList.Last().EndX - BeamList.Last().StartX;
            } else {
                BeamList.RemoveAt(BeamList.Count - 1);
            }
        }
    }

    // 前台保证所有跨距正确，此处更新坐标即可,更新完立柱需要更新主梁
    public void UpdatePostX() {
        if (PostList == null) return;

        PostList[0].Num = 1;
        PostList[0].X = PostList[0].LeftSpan;
        PostList[0].StartX = PostList[0].X - PostList[0].LeftToBeam;
        PostList[0].EndX = PostList[0].X + PostList[0].RightToBeam;
        for (var i = 1; i < PostList.Count; i++) {
            PostList[i].Num = i + 1;
            PostList[i].X = PostList[i - 1].X + PostList[i].LeftSpan;
            PostList[i].StartX = PostList[i].X - PostList[i].LeftToBeam;
            PostList[i].EndX = PostList[i].X + PostList[i].RightToBeam;
        }

        while (PostList.Last().X >= SystemLength - LeftRemind - RightRemind) {
            PostList.RemoveAt(PostList.Count - 1);
        }

        PostList.Last().RightSpan = SystemLength - LeftRemind - RightRemind - PostList.Last().X;

        // 立柱坐标更新完毕，更新主梁的长度与首伟相连
        //if (!HasSlew) return; // 如果有电机则更新主梁信息
        UpdateDrivePost();
        UpdateBeamX();

        if (DriveGap > 0) {
            InitPurlinWithDriveGap();
        } else {
            InitPurlinWithOutDriveGap();
        }
    }


    // 修正主梁属性,前台续保证主梁大致有序
    private void UpdateBeamData() {
        if (BeamList == null || PostList == null) return;

        var drivePostList = PostList.Where(post => post.IsDrive && post.IsMotor).ToList();
        // 使用数据结构队列
        var drivePostIndex = 0;
        var beamIndex = 0;
        var itemQueue = new Queue<ITrackerItemModel>();
        while (drivePostIndex < drivePostList.Count || beamIndex < BeamList.Count) {
            var drivePost = drivePostIndex < drivePostList.Count ? drivePostList[drivePostIndex] : null;
            var beam = beamIndex < BeamList.Count ? BeamList[beamIndex] : null;
            if (drivePost == null) {
                itemQueue.Enqueue(beam!);
                beamIndex++;
            } else if (beam == null) {
                itemQueue.Enqueue(drivePost!);
                drivePostIndex++;
            } else {
                if (beam.EndX <= drivePost.StartX) { //当前主梁在立柱前面，此处与初始化时不同，不考虑最小主梁长度，主梁入队
                    itemQueue.Enqueue(beam);
                    beamIndex++;
                } else if (beam.EndX > drivePost.StartX // 当前
                        && beam.EndX <= drivePost.EndX) {
                    // 将当前主梁的尾部接上驱动立柱，并调整后续主梁
                    beam.EndX = drivePost.StartX;
                    beam.Length = beam.EndX - beam.StartX;
                    while (beamIndex + 1 < BeamList.Count && BeamList[beamIndex + 1].EndX <= drivePost.EndX) {
                        BeamList.RemoveAt(beamIndex + 1);
                    }

                    itemQueue.Enqueue(beam);
                    beamIndex++;
                } else if (beam.EndX > drivePost.EndX && beam.StartX <= drivePost.StartX) {
                    // 主梁在驱动立柱上方，需要将主梁分成两部分，特别短的先不管(规定主梁最小长度)，后面整理队列的时候处理，主梁入队
                    var newBeam = BeamMapper.Map<BeamModel, BeamModel>(beam); //
                    newBeam.EndX = drivePost.StartX;
                    newBeam.Length = newBeam.EndX - newBeam.StartX;
                    beam.StartX = drivePost.EndX;
                    //beam.Length    = beam.EndX - beam.StartX; // 此处无需更新主梁长度，因为可能是驱动立柱位置发生变化
                    BeamList.Insert(beamIndex, newBeam);
                    itemQueue.Enqueue(newBeam);
                } else { // 立柱在前面
                    itemQueue.Enqueue(drivePost);
                    drivePostIndex++;
                    beam.StartX = Math.Max(beam.StartX, drivePost.EndX);
                    //beam.Length = beam.EndX - beam.StartX;
                }
            }
        }

        // 将主梁和立柱依次相连
        while (itemQueue.Count > 0) {
            var item1 = itemQueue.Dequeue();
            if (itemQueue.Count <= 0) continue;
            var item2 = itemQueue.Peek();
            item1.NextItem = item2;
            item2.PreItem = item1;
        }

        BeamList.Last().NextItem = null;
        BeamList.First().PreItem = null;
        // 遍历主梁修改间隙
        for (var i = 0; i < BeamList.Count; i++) {
            BeamList[i].Num = i + 1;
            switch (BeamList[i].NextItem) {
                case BeamModel beam:
                    BeamList[i].RightToNext = BeamGap;
                    beam.LeftToPre = BeamGap;
                    // 修正主梁坐标
                    var dist = beam.StartX - BeamList[i].EndX;
                    BeamList[i].EndX += Math.Abs(dist - BeamGap) / 2;
                    BeamList[i].Length = BeamList[i].EndX - BeamList[i].StartX;
                    beam.StartX -= Math.Abs(dist - BeamGap) / 2;
                    beam.Length = beam.EndX - beam.StartX;
                    break;
                case PostModel drivePost:
                    BeamList[i].RightToNext = drivePost.LeftToBeam + drivePost.RightToBeam;
                    //BeamList[i].EndX 
                    if (i < BeamList.Count - 1) {
                        BeamList[i + 1].LeftToPre = drivePost.LeftToBeam + drivePost.RightToBeam;
                    }

                    break;
                default: // 当前主梁为最后一个的情况
                    BeamList[i].RightToNext = 0;
                    break;
            }
        }
    }

    // 调整驱动立柱位置
    private void UpdateDrivePost() {
        if (PostList == null || DriveGap <= 0) return;
        var drivePostList = PostList.Where(post => post.IsDrive).ToList();
        // 调整立柱
        for (var i = 0; i < drivePostList.Count; i++) {
            var drivePost = drivePostList[i];
            // 计算当前驱动立柱前方可以放多少组件
            var drivePostToFirstPurlin = drivePost.X + ModuleGapAxis / 2 - (DriveGap - ModuleGapAxis) / 2;
            drivePostToFirstPurlin -= i * (DriveGap - ModuleGapAxis);
            var purlinNum = drivePostToFirstPurlin / (ModuleWidth + ModuleGapAxis);
            var modifyFactor = purlinNum - (int)purlinNum;
            if (modifyFactor <= 0.5) { // 靠前一个檩条近
                drivePost.X -= modifyFactor * (ModuleWidth + ModuleGapAxis);
                drivePost.LeftSpan -= modifyFactor * (ModuleWidth + ModuleGapAxis);
                drivePost.RightSpan += modifyFactor * (ModuleWidth + ModuleGapAxis);
            } else {
                drivePost.X += (1 - modifyFactor) * (ModuleWidth + ModuleGapAxis);
                drivePost.LeftSpan += (1 - modifyFactor) * (ModuleWidth + ModuleGapAxis);
                drivePost.RightSpan -= (1 - modifyFactor) * (ModuleWidth + ModuleGapAxis);
            }

            drivePost.StartX = drivePost.X - drivePost.LeftToBeam;
            drivePost.EndX = drivePost.X + drivePost.RightToBeam;
            if (drivePost.Num > 1) {
                PostList[drivePost.Num - 2].RightSpan = drivePost.LeftSpan;
            }

            if (drivePost.Num < PostList.Count) {
                PostList[drivePost.Num].LeftSpan = drivePost.RightSpan;
            }
        }
    }

    #endregion
}