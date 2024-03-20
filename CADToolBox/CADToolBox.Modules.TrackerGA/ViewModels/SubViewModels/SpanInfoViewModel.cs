using System;
using System.Collections;
using CADToolBox.Shared.Models.UIModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Properties.Langs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using AutoMapper;
using CADToolBox.Modules.TrackerGA.Messages;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CADToolBox.Shared.Models.CADModels.Implement;
using CADToolBox.Shared.Models.UIModels.Implement;
using CADToolBox.Shared.Models.UIModels.Interface;
using CADToolBox.Shared.Tools;
using System.Reflection;
using CADToolBox.Shared.Models.CADModels.Interface;

namespace CADToolBox.Modules.TrackerGA.ViewModels.SubViewModels;

public partial class SpanInfoViewModel : ViewModelBase {
    #region 字段与属性

    [ObservableProperty]
    private TrackerModel? _trackerModel;

    [ObservableProperty]
    private ObservableCollection<PostInfo>? _postInfos;

    [ObservableProperty]
    private ObservableCollection<BeamInfo>? _beamInfos;

    [ObservableProperty]
    private PostModel _defaultPostModel = new() {
                                                    Num = 1,
                                                    IsDrive = false,
                                                    SectionType = "W型钢",
                                                    Material = "Q355"
                                                };

    [ObservableProperty]
    private BeamModel _defaultBeamModel = new() {
                                                    Num = 1,
                                                    SectionType = "方管",
                                                    Material = "Q355"
                                                };

    [ObservableProperty]
    private List<string> _sectionMaterial = ["Q235", "Q355", "Q420", "Q500"];

    [ObservableProperty]
    private int _filterTextIndex;

    private readonly MapperConfiguration _postModelConfig = new(cfg => cfg.CreateMap<PostModel, PostModel>());
    private readonly MapperConfiguration _beamModelConfig = new(cfg => cfg.CreateMap<BeamModel, BeamModel>());

    private IMapper PostInfoMapper => _postModelConfig.CreateMapper();
    private IMapper BeamInfoMapper => _beamModelConfig.CreateMapper();

    // 点击时是选取一行函数单个单元格
    [ObservableProperty]
    private DataGridSelectionUnit _currentSelectionUnit = DataGridSelectionUnit.CellOrRowHeader;

    #endregion


    #region 构造方法

    public SpanInfoViewModel() {
        // 动态获取当前画布的大小
        WeakReferenceMessenger.Default.Register<WindowSizeChangedMessage>(this, (obj, message) => {
                                                                                    CanvasHeight = message.CanvasHeight;
                                                                                    CanvasWidth = message.CanvasWidth;
                                                                                    // 画图
                                                                                    UpdateSystemDraw();
                                                                                });
        // 进入该界面时所有的坐标默认是正确的
        TrackerModel = TrackerApp.Current.TrackerModel!;
        TrackerModel.PropertyChanged += OnTrackerModelChanged;

        PostInfos = [];
        // 初始化立柱信息
        if (TrackerModel?.PostList != null) {
            foreach (var newPostInfo in TrackerModel.PostList.Select(postModel => new PostInfo(postModel))) {
                newPostInfo.LeftSpanChanged += OnLeftSpanChanged;
                newPostInfo.RightSpanChanged += OnRightSpanChanged;
                newPostInfo.IsDriveChanged += OnIsDriveChanged;
                PostInfos.Add(newPostInfo);
            }
        }

        BeamInfos = [];
        // 初始化主梁信息
        if (TrackerModel?.BeamList != null) {
            foreach (var newBeamInfo in TrackerModel.BeamList.Select(beamModel => new BeamInfo(beamModel))) {
                newBeamInfo.LengthChanged += OnBeamLengthChanged;
                BeamInfos.Add(newBeamInfo);
            }
        }

        // 初始化绘图
        UpdateSystemDraw();
    }

    #endregion


    #region 其他Relaycommand

    // 是否显示展开想
    [RelayCommand]
    private void SwitchDetailVisible(ITrackerItemInfo currentInfo) {
        currentInfo.IsDetailsVisible = !currentInfo.IsDetailsVisible;
    }

    #endregion

    #region 操作立柱表格的RelayCommand

    [RelayCommand]
    private void AddPost(PostInfo currentPostInfo) {
        var currentPost = currentPostInfo.PostModel;
        var insertPost = PostInfoMapper.Map<PostModel, PostModel>(currentPost);
        currentPost.RightSpan = insertPost.LeftSpan;
        if (currentPost.Num == TrackerModel!.PostList!.Count) {
            TrackerModel!.PostList!.Add(insertPost);
        } else {
            insertPost.RightSpan = TrackerModel!.PostList![currentPost.Num].LeftSpan;
            TrackerModel!.PostList!.Insert(currentPost.Num, insertPost);
        }

        TrackerModel!.UpdatePostX();
        UpdatePostInfos();
    }

    [RelayCommand]
    private void DeletePost(PostInfo currentPostInfo) {
        if (PostInfos!.Count <= 1) {
            MessageBox.Show("至少需要一个立柱");
        }

        TrackerModel!.PostList!.RemoveAt(currentPostInfo.Num - 1);
        TrackerModel!.UpdatePostX();
        UpdatePostInfos();
    }

    // 前台选中的立柱发生改变时让立柱线段变粗
    [RelayCommand]
    private void PostInfoSelectionChanged(IList selectedItems) {
        if (PostInfos != null)
            foreach (var postInfo in PostInfos) {
                postInfo.IsSelected = false;
            }

        foreach (var selectedItem in selectedItems) {
            if (selectedItem is PostInfo selectedPost) selectedPost.IsSelected = true;
        }

        UpdateSystemDraw();
    }

    #endregion

    #region 操作主梁表格的Relaycommand

    // 如果右侧没有驱动立柱可以在后面直接添加，变成分割的逻辑
    [RelayCommand]
    private void AddBeam(BeamInfo currentBeamInfo) {
        var currentBeam = currentBeamInfo.BeamModel;
        var insertBeam = BeamInfoMapper.Map<BeamModel, BeamModel>(currentBeam);
        var lastItem = currentBeam.NextItem;
        while (lastItem != null && lastItem is not PostModel) {
            lastItem = lastItem.NextItem;
        }

        if (lastItem is PostModel) { // 如果当前主梁后方连着驱动立柱，只能将主梁等分,这种情况等于没有修改元主梁数组的坐标
            const double radio = 0.5;
            var tempLength = currentBeam.Length;
            currentBeam.Length = Convert.ToInt32(tempLength * radio / 100) * 100;
            currentBeam.EndX = currentBeam.StartX + currentBeam.Length;
            insertBeam.Length = tempLength - currentBeam.Length - TrackerModel!.BeamGap;
            insertBeam.StartX = insertBeam.EndX - insertBeam.Length;
            currentBeam.NextItem = insertBeam;
            insertBeam.PreItem = currentBeam;
            if (currentBeamInfo.Num < TrackerModel!.BeamList!.Count) {
                TrackerModel!.BeamList!.Insert(currentBeamInfo.Num, insertBeam);
            } else {
                TrackerModel!.BeamList!.Add(insertBeam);
            }
        } else {                                              // 如果当前主梁后面没有驱动立柱,这种情况需要从插入的位置开始更新主梁坐标
            var tempBeam = currentBeam.NextItem as BeamModel; // 将当前主梁后面的主梁缓存
            currentBeam.NextItem = insertBeam;
            currentBeam.RightToNext = TrackerModel!.BeamGap;
            insertBeam.PreItem = currentBeam;
            insertBeam.LeftToPre = TrackerModel!.BeamGap;
            insertBeam.NextItem = tempBeam;
            insertBeam.RightToNext = TrackerModel!.BeamGap;
            if (tempBeam != null) {
                tempBeam.PreItem = insertBeam;
                tempBeam.LeftToPre = TrackerModel!.BeamGap;
            }

            if (currentBeamInfo.Num < TrackerModel!.BeamList!.Count) {
                TrackerModel!.BeamList!.Insert(currentBeamInfo.Num, insertBeam);
            } else {
                TrackerModel!.BeamList!.Add(insertBeam);
            }

            for (var i = currentBeamInfo.Num; i < TrackerModel!.BeamList!.Count; i++) {
                TrackerModel!.BeamList[i].StartX = TrackerModel!.BeamList[i - 1].EndX + TrackerModel!.BeamList[i].LeftToPre;
                TrackerModel!.BeamList[i].EndX = TrackerModel!.BeamList[i].StartX + TrackerModel!.BeamList[i].Length;
            }
        }

        TrackerModel!.UpdateBeamX();
        UpdateBeamInfos();
    }

    [RelayCommand]
    private void DeleteBeam(BeamInfo currentBeamInfo) {
        if (BeamInfos!.Count <= 1) {
            MessageBox.Show("主梁数量最少为1");
            return;
        }

        var currentBeam = currentBeamInfo.BeamModel;
        var lastItem = currentBeam.NextItem;
        while (lastItem != null && lastItem is not PostModel) {
            lastItem = lastItem.NextItem;
        }

        if (lastItem is PostModel) { // 如果当前主梁后方连着驱动立柱，只能将主梁合并,这种情况等于没有修改元主梁数组的坐标
            if (currentBeam.NextItem is PostModel) {
                MessageBox.Show("驱动立柱左侧最近主梁无法删除");
                return;
            }

            // 此时后方肯定为主梁
            var nextBeam = currentBeam.NextItem as BeamModel;
            nextBeam!.PreItem = currentBeam.PreItem;
            nextBeam.LeftToPre = currentBeam.LeftToPre;
            nextBeam.StartX = currentBeam.StartX;
            nextBeam.Length = nextBeam.EndX - nextBeam.StartX;
            if (currentBeam.PreItem != null) {
                currentBeam.PreItem.NextItem = nextBeam;
            }
        } else { // 如果当前主梁后面没有驱动立柱,需要将后方主梁前移，需要更新坐标
            // 此时后方肯定全为主梁主梁
            if (currentBeam.NextItem is BeamModel nextBeam) {
                nextBeam.StartX = currentBeam.StartX;
                nextBeam.LeftToPre = currentBeam.LeftToPre;
                nextBeam.EndX = nextBeam.StartX + nextBeam.Length;
                while (nextBeam.NextItem != null) {
                    var tempBeam = nextBeam.NextItem as BeamModel;
                    tempBeam!.StartX = nextBeam.EndX + nextBeam.RightToNext;
                    tempBeam.EndX = tempBeam.StartX + tempBeam.Length;
                    nextBeam = tempBeam;
                }
            }
        }

        TrackerModel!.BeamList!.RemoveAt(currentBeamInfo.Num - 1); // 移除当前主梁

        TrackerModel!.UpdateBeamX();
        UpdateBeamInfos();
    }


    // 前台选中的主梁发生改变时
    [RelayCommand]
    private void BeamInfoSelectionChanged(IList selectedItems) {
        if (BeamInfos != null)
            foreach (var beamInfo in BeamInfos) {
                beamInfo.IsSelected = false;
            }

        foreach (var selectedItem in selectedItems) {
            if (selectedItem is BeamInfo selectedBeam) selectedBeam.IsSelected = true;
        }

        UpdateSystemDraw();
    }

    #endregion

    #region 普通方法区

    // 更新前台的主梁信息表
    private void UpdateBeamInfos() {
        if (TrackerModel!.BeamList == null || BeamInfos == null) return;


        // 实际主梁列表与主梁信息列表相同的部分
        var count = Math.Min(TrackerModel!.BeamList.Count, BeamInfos.Count);
        for (var i = 0; i < count; i++) {
            BeamInfos[i].BeamModel = TrackerModel.BeamList[i];
            BeamInfos[i].OnBeamModelChanged();
        }

        if (TrackerModel!.BeamList.Count >= BeamInfos.Count) {
            // 展示的比实际的少需要增加
            for (var i = count; i < TrackerModel!.BeamList.Count; i++) {
                var newBeamInfo = new BeamInfo(TrackerModel!.BeamList[i]);
                newBeamInfo.LengthChanged += OnBeamLengthChanged;
                BeamInfos.Add(newBeamInfo);
                newBeamInfo.OnBeamModelChanged();
            }
        } else {
            // 展示的比实际的多需要将末尾的减少
            while (BeamInfos.Count > TrackerModel!.BeamList.Count) {
                BeamInfos.RemoveAt(BeamInfos.Count - 1);
            }
        }

        UpdateSystemDraw();
    }

    // 更新前台立柱信息表
    private void UpdatePostInfos() {
        if (TrackerModel!.PostList == null || PostInfos == null) return;
        var count = Math.Min(TrackerModel!.PostList.Count, PostInfos.Count);
        for (var i = 0; i < count; i++) {
            PostInfos[i].PostModel = TrackerModel.PostList[i];
            PostInfos[i].OnPostModelChanged();
        }

        if (TrackerModel!.PostList.Count >= PostInfos.Count) {
            for (var i = count; i < TrackerModel!.PostList.Count; i++) {
                var newPostInfo = new PostInfo(TrackerModel!.PostList[i]);
                newPostInfo.IsDriveChanged += OnIsDriveChanged;
                newPostInfo.LeftSpanChanged += OnLeftSpanChanged;
                newPostInfo.RightSpanChanged += OnRightSpanChanged;
                PostInfos.Add(newPostInfo);
            }
        } else {
            for (var i = PostInfos.Count - 1; i >= count; i--) {
                PostInfos.RemoveAt(i);
            }
        }

        UpdateBeamInfos(); // 更新完立柱需要更新主梁，绘图在更新主梁之后完成
    }

    #endregion

    #region 处理事件

    #region 立柱事件

    private void OnIsDriveChanged(object sender, EventArgs e) {
        //MessageBox.Show("立柱类型发生改变");
        var modifiedPost = (PostInfo)sender;

        if (modifiedPost.IsDrive) { // 由普通立柱变成驱动立柱
            if (!TrackerModel!.HasSlew) modifiedPost.IsMotor = false;
            // 如果该处有电机开断需要将上面的主梁打断成两份
            modifiedPost.IsMotor = true;
            modifiedPost.LeftToBeam = 75;
            modifiedPost.RightToBeam = 75;
            // 找到上面的主梁，将其分成两段接入
        } else { // 由驱动立柱变成普通立柱
            // 将两侧主梁开断都置为0
            modifiedPost.IsMotor = false;
            modifiedPost.LeftToBeam = 0;
            modifiedPost.RightToBeam = 0;
            if (TrackerModel!.HasSlew) { // 如果有点击需要修改驱动两侧的主梁，合并为同一个主梁
                var leftBeam = modifiedPost.PostModel.PreItem as BeamModel;
                var rightBeam = modifiedPost.PostModel.NextItem as BeamModel;
                modifiedPost.PostModel.PreItem = null;
                modifiedPost.PostModel.NextItem = null;
                if (leftBeam != null) {
                    leftBeam.EndX = modifiedPost.X - TrackerModel!.BeamGap / 2;
                    leftBeam.Length = leftBeam.EndX - leftBeam.StartX;
                    leftBeam.RightToNext = TrackerModel!.BeamGap;
                    leftBeam.NextItem = rightBeam ?? null;
                }

                if (rightBeam != null) {
                    rightBeam.StartX = modifiedPost.X + TrackerModel!.BeamGap / 2;
                    rightBeam.Length = rightBeam.EndX - rightBeam.StartX;
                    rightBeam.LeftToPre = TrackerModel!.BeamGap;
                    rightBeam.PreItem = leftBeam ?? null;
                }
            }
        }

        TrackerModel!.UpdatePostX();
        UpdatePostInfos();
    }

    private void OnLeftSpanChanged(object sender, EventArgs e) {
        //MessageBox.Show("左侧跨距发生改变");
        var modifiedPost = (PostInfo)sender;
        if (modifiedPost.Num == 1) return;
        PostInfos![modifiedPost.Num - 2].RightSpan = modifiedPost.LeftSpan;
        TrackerModel!.UpdatePostX();
        UpdatePostInfos();
    }

    private void OnRightSpanChanged(object sender, EventArgs e) {
        //MessageBox.Show("右侧跨距发生改变");
        var modifiedPost = (PostInfo)sender;
        if (modifiedPost.Num == PostInfos!.Count) return;
        PostInfos![modifiedPost.Num].LeftSpan = modifiedPost.RightSpan;
        TrackerModel!.UpdatePostX();
        UpdatePostInfos();
    }

    #endregion

    #region 主梁事件，主梁只有长度变化影响绘图，暂不考虑

    //当主梁分段长度发生变化时更新主梁分段长度，不对数组进行增减操作
    private void OnBeamLengthChanged(object sender, EventArgs e) {
        if (BeamInfos == null) {
            return;
        }

        var currentBeamInfo = (BeamInfo)sender;
        var currentBeam = currentBeamInfo.BeamModel;
        var lastItem = currentBeam.NextItem;
        while (lastItem is BeamModel) {
            lastItem = lastItem.NextItem;
        }

        currentBeam.EndX = currentBeam.StartX + currentBeam.Length;
        if (lastItem == null) { // 当前修改的主梁后方全部都是主梁
            var nextBeam = currentBeam.NextItem;
            while (nextBeam != null) {
                var beamModel = nextBeam as BeamModel;
                beamModel!.StartX = currentBeam.EndX + beamModel.LeftToPre;
                beamModel.EndX = beamModel.StartX + beamModel.Length;
                currentBeam = beamModel;
                nextBeam = nextBeam.NextItem;
            }
        } else { // 当前修改的主梁后方有驱动立柱
            //var drivePost = lastItem as PostModel;
            var nextItem = currentBeam.NextItem;
            if (nextItem is PostModel drivePost) {
                currentBeam.EndX = drivePost.StartX;
                currentBeam.Length = currentBeam.EndX - currentBeam.StartX;
            } else {
                var nextBeam = nextItem as BeamModel;
                if (currentBeam.EndX + TrackerModel!.BeamGap <= nextBeam!.EndX - TrackerModel!.MinBeamLength) {
                    // 这种情况下长度修改成功
                    nextBeam.StartX = currentBeam.EndX + TrackerModel!.BeamGap;
                    nextBeam.Length = nextBeam.EndX - nextBeam.StartX;
                } else { // 这种情况下需要修正当前主梁的长度
                    nextBeam.Length = TrackerModel!.MinBeamLength;
                    nextBeam.StartX = nextBeam.EndX - nextBeam.Length;

                    currentBeam.EndX = nextBeam.StartX - TrackerModel!.BeamGap;
                    currentBeam.Length = currentBeam.EndX - currentBeam.StartX;
                }
            }
        }

        TrackerModel!.UpdateBeamX();
        UpdateBeamInfos();
    }

    #endregion


    // 当TrackerModel属性发生改变时也要重新绘图，主要是末端余量的变化
    private void OnTrackerModelChanged(object sender, PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(TrackerModel.HasSlew):
                if (TrackerModel!.HasSlew) { // 驱动类型变成回转
                    TrackerModel.DriveType = "回转";
                    TrackerModel.DriveGap = 400;
                    foreach (var postInfo in PostInfos!) {
                        if (postInfo.IsDrive) {
                            postInfo.IsMotor = true;
                            postInfo.LeftToBeam = 75;
                            postInfo.RightToBeam = 75;
                        }
                    }
                } else { // 驱动类型变成推杆
                    TrackerModel.DriveType = "推杆";
                    TrackerModel.DriveGap = 0;

                    foreach (var postInfo in PostInfos!) {
                        postInfo.LeftToBeam = 0;
                        postInfo.RightToBeam = 0;
                        if (!postInfo.IsDrive) continue;
                        postInfo.IsMotor = false;
                        var postModel = postInfo.PostModel;
                        if (postModel.PreItem == null) {
                            if (postModel.NextItem != null) {
                                postModel.NextItem.PreItem = null;
                            }
                        } else if (postModel.NextItem == null) {
                            if (postModel.PreItem != null) {
                                postModel.PreItem.NextItem = null;
                            }
                        } else {
                            postModel.PreItem.NextItem = postModel.NextItem;
                            postModel.NextItem.PreItem = postModel.PreItem;
                        }

                        postModel.PreItem = null;
                        postModel.NextItem = null;
                    }
                }

                TrackerModel!.UpdatePostX();
                UpdatePostInfos();
                break;
            case nameof(TrackerModel.BeamGap):
                TrackerModel!.UpdateBeamX();
                UpdateBeamInfos();
                UpdateSystemDraw();
                break;
            case nameof(TrackerModel.DriveGap):
                if (TrackerModel!.DriveGap > 0) {
                    TrackerModel!.InitPurlinWithDriveGap();
                    UpdateSystemDraw();
                } else {
                    TrackerModel!.InitPurlinWithOutDriveGap();
                    UpdateSystemDraw();
                }

                break;
        }
    }

    #endregion

    #region 画图用

    public double CanvasHeight { get; private set; }

    public double CanvasWidth { get; private set; }

    [ObservableProperty]
    private ObservableCollection<CanvasPostLine> _postLines = [];

    [ObservableProperty]
    private ObservableCollection<CanvasBeamLine> _beamLines = [];

    [ObservableProperty]
    private ObservableCollection<CanvasModuleLine> _moduleLines = [];

    private void UpdateSystemDraw() {
        var maxY = TrackerModel!.BeamCenterToGround + TrackerModel.BeamHeight * (1 + TrackerModel.BeamRadio) + TrackerModel.PurlinHeight + TrackerModel.ModuleHeight;
        var maxX = Math.Max(Math.Max(TrackerModel!.SystemLength, PostInfos == null ? 0 : PostInfos.Last().X), (BeamInfos == null || BeamInfos.Count == 0) ? 0 : BeamInfos.Last().EndX);

        const double scaleRadioX = 0.95;
        const double scaleRadioY = 0.95;

        var scaleX = maxX == 0 ? 0 : scaleRadioX * CanvasWidth / maxX;
        var scaleY = maxY == 0 ? 0 : scaleRadioY * CanvasHeight / maxY;

        var startX = CanvasWidth * (1 - scaleRadioX) / 2;


        // 画立柱部分
        if (PostInfos != null) {
            PostLines = [];
            foreach (var postInfo in PostInfos) {
                PostLines.Add(new CanvasPostLine(postInfo, (postInfo.X + TrackerModel.LeftRemind) * scaleX + startX, CanvasHeight, (postInfo.X + TrackerModel.LeftRemind) * scaleX + startX, CanvasHeight - TrackerModel.BeamCenterToGround * scaleY));
            }
        }

        // 画主梁部分
        if (BeamInfos != null) {
            BeamLines = [];
            var randomColors = SolidColorBrushGenerator.GenerateSolidColorBrushes(BeamInfos.Count);
            for (var i = 0; i < BeamInfos.Count; i++) {
                BeamLines.Add(new CanvasBeamLine(BeamInfos[i], randomColors[i], (BeamInfos[i].StartX + TrackerModel.LeftRemind) * scaleX + startX, CanvasHeight - TrackerModel.BeamCenterToGround * scaleY, (BeamInfos[i].EndX + TrackerModel.LeftRemind) * scaleX + startX, CanvasHeight - TrackerModel.BeamCenterToGround * scaleY));
            }
        }

        // 画组件部分
        if (TrackerModel!.PurlinList != null) {
            ModuleLines = [];
            var moduleY = TrackerModel.BeamCenterToGround + TrackerModel.BeamHeight * (1 + TrackerModel.BeamRadio) + TrackerModel.PurlinHeight;
            for (var i = 0; i < TrackerModel.PurlinList.Count - 1; i++) {
                var leftPurlin = TrackerModel.PurlinList[i];
                var rightPurlin = TrackerModel.PurlinList[i + 1];
                if (rightPurlin.Type == -1) continue; // 右侧檩条为左檩条跳出循环

                ModuleLines.Add(new CanvasModuleLine((leftPurlin.X + TrackerModel.ModuleGapAxis / 2 + TrackerModel.LeftRemind) * scaleX + startX, CanvasHeight - moduleY * scaleY, (rightPurlin.X - TrackerModel.ModuleGapAxis / 2 + TrackerModel.LeftRemind) * scaleX + startX, CanvasHeight - moduleY * scaleY));
            }
        }
    }

    #endregion
}