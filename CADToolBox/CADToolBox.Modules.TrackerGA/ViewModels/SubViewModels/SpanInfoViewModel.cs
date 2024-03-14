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
                                                    Num         = 1,
                                                    IsDrive     = false,
                                                    SectionType = "W型钢",
                                                    Material    = "Q355"
                                                };

    [ObservableProperty]
    private BeamModel _defaultBeamModel = new() {
                                                    Num         = 1,
                                                    SectionType = "方管",
                                                    Material    = "Q355"
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
        WeakReferenceMessenger.Default.Register<WindowSizeChangedMessage>(this, (obj,
                                                                              message) => {
                                                                              CanvasHeight = message.CanvasHeight;
                                                                              CanvasWidth  = message.CanvasWidth;
                                                                              // 画图
                                                                              UpdateSystemDraw();
                                                                          });
        // 进入该界面时所有的坐标默认是正确的
        TrackerModel                 =  TrackerApp.Current.TrackerModel!;
        TrackerModel.PropertyChanged += OnTrackerModelChanged;

        PostInfos = [];
        // 初始化立柱信息
        if (TrackerModel?.PostList != null) {
            foreach (var newPostInfo in TrackerModel.PostList.Select(postModel => new PostInfo(postModel))) {
                newPostInfo.LeftSpanChanged += (obj,
                                                _) => {
                                                   var modifiedPost = (PostInfo)obj;
                                                   if (modifiedPost.Num == 1) return;
                                                   PostInfos![modifiedPost.Num - 2].RightSpan = modifiedPost.LeftSpan;
                                                   UpdatePostInfos();
                                               };
                newPostInfo.RightSpanChanged += (obj,
                                                 _) => {
                                                    var modifiedPost = (PostInfo)obj;
                                                    if (modifiedPost.Num == PostInfos.Count) return;
                                                    PostInfos![modifiedPost.Num].LeftSpan = modifiedPost.RightSpan;
                                                    UpdatePostInfos();
                                                };
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


    #region 通用Relaycommand

    [RelayCommand]
    private void SwitchDetailVisible(ITrackerItemInfo currentInfo) {
        currentInfo.IsDetailsVisible = !currentInfo.IsDetailsVisible;
    }

    #endregion


    #region 操作立柱表格的Relaycommand

    [RelayCommand]
    private void AddPost(PostInfo currentPostInfo) {
        var currentPost = currentPostInfo.PostModel;
        var insertPost  = PostInfoMapper.Map<PostModel, PostModel>(currentPost);
        if (currentPostInfo.Num == PostInfos!.Count) {
            TrackerModel!.PostList!.Insert(PostInfos.Count - 1, insertPost);
        } else {
            TrackerModel!.PostList!.Insert(currentPostInfo.Num, insertPost);
        }

        TrackerModel!.InitPost();
        UpdatePostInfos();
    }

    [RelayCommand]
    private void DeletePost(PostInfo currentPostInfo) {
        if (PostInfos!.Count <= 1) {
            MessageBox.Show("至少需要一个立柱");
        }

        TrackerModel!.PostList!.RemoveAt(currentPostInfo.Num - 1);
        TrackerModel!.InitPost();
        UpdatePostInfos();
    }

    // 前台选中的立柱发生改变时
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

    [RelayCommand]
    private void AddBeam(BeamInfo currentBeamInfo) {
        var currentBeam = currentBeamInfo.BeamModel;
        var insertBeam  = BeamInfoMapper.Map<BeamModel, BeamModel>(currentBeam);
        switch (currentBeam.NextItem) {
            case BeamModel: // 当前主梁下一个也是主梁
                currentBeam.NextItem = insertBeam;
                insertBeam.PreItem   = currentBeam;
                TrackerModel!.BeamList!.Insert(currentBeamInfo.Num, insertBeam);
                break;
            case PostModel drivePost:
                insertBeam.PreItem = drivePost;
                TrackerModel!.BeamList!.Insert(currentBeamInfo.Num, insertBeam);
                break;
            default: // 最后一个主梁
                currentBeam.NextItem = insertBeam;
                insertBeam.PreItem   = currentBeam;
                insertBeam.NextItem  = null;
                TrackerModel!.BeamList!.Insert(BeamInfos!.Count - 1, insertBeam);
                break;
        }

        UpdateBeamInfos();
    }

    [RelayCommand]
    private void DeleteBeam(BeamInfo currentBeamInfo) {
        if (BeamInfos!.Count <= 1) {
            MessageBox.Show("主梁数量最少为1");
            return;
        }

        var flag        = false;
        var currentBeam = currentBeamInfo.BeamModel;
        switch (currentBeam.NextItem) {
            case BeamModel beamModel: // 当前主梁下一个也是主梁
                if (currentBeam.PreItem != null) {
                    currentBeam.PreItem.NextItem = beamModel;
                    beamModel.PreItem            = currentBeam.PreItem;
                }

                IItemModel tempModel = beamModel;
                while (tempModel != null && tempModel is not PostModel) {
                    tempModel = tempModel.NextItem;
                }

                if (tempModel is PostModel) { // 当前主梁后面有驱动立柱，这种情况主梁不能往前移动
                    // 更新前后关系与长度信息
                    beamModel.StartX = currentBeam.StartX;
                    beamModel.Length = beamModel.EndX - beamModel.StartX;
                } else {
                    beamModel.StartX = currentBeam.StartX;
                    beamModel.EndX   = beamModel.StartX + beamModel.Length;
                }

                flag = true;
                break;
            case PostModel drivePost:
                switch (currentBeam.PreItem) {
                    case PostModel:
                        MessageBox.Show("当前主梁不可删除"); // 两个驱动立柱之间只有一个主梁时不可删除
                        break;
                    case BeamModel beamModel:
                        beamModel.NextItem = drivePost;
                        beamModel.EndX     = currentBeam.EndX;
                        beamModel.Length   = beamModel.EndX - beamModel.StartX;
                        flag               = true;
                        break;
                    default:                         // 已经为第一个主梁
                        MessageBox.Show("当前主梁不可删除"); // 驱动立柱前面只有一个主梁的情况
                        break;
                }

                break;
            default: // 最后一个主梁
                switch (currentBeam.PreItem) {
                    case BeamModel beamModel:
                        beamModel.NextItem = null;
                        flag               = true;
                        break;
                    default: // 驱动立柱后面只有一个主梁的情况
                        MessageBox.Show("当前主梁不可删除");
                        break;
                }

                break;
        }

        if (!flag) return;
        TrackerModel!.BeamList!.RemoveAt(currentBeamInfo.Num - 1);
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

    // 后台的主梁列表更新后更新前台显示的主梁信息列表
    private void UpdateBeamInfos() {
        if (TrackerModel!.BeamList == null) {
            return;
        }

        if (BeamInfos == null) {
            return;
        }

        TrackerModel!.UpdateBeamX();
        // 实际主梁列表与主梁信息列表相同的部分
        var count = Math.Min(TrackerModel!.BeamList.Count, BeamInfos.Count);
        for (var i = 0; i < count; i++) {
            BeamInfos[i].BeamModel     =  TrackerModel.BeamList[i];
            BeamInfos[i].LengthChanged -= OnBeamLengthChanged;
            BeamInfos[i].LengthChanged += OnBeamLengthChanged;
        }

        if (TrackerModel!.BeamList.Count >= BeamInfos.Count) {
            // 展示的比实际的少需要增加
            for (var i = count; i < TrackerModel!.BeamList.Count; i++) {
                var newBeamInfo = new BeamInfo(TrackerModel!.BeamList[i]);
                newBeamInfo.LengthChanged += OnBeamLengthChanged;
                BeamInfos.Add(newBeamInfo);
            }
        } else {
            // 展示的比实际的多需要将末尾的减少
            for (var i = BeamInfos.Count - 1; i >= count; i--) {
                BeamInfos.RemoveAt(i);
            }
        }

        UpdateSystemDraw();
    }


    private void UpdatePostInfos() {
        if (TrackerModel!.PostList == null || PostInfos == null) return;

        TrackerModel!.UpdatePostX();
        var count = Math.Min(TrackerModel!.PostList.Count, PostInfos.Count);
        for (var i = 0; i < count; i++) {
            PostInfos[i].PostModel = TrackerModel.PostList[i];
        }

        if (TrackerModel!.PostList.Count >= PostInfos.Count) {
            for (var i = count; i < TrackerModel!.PostList.Count; i++) {
                PostInfos.Add(new PostInfo(TrackerModel!.PostList[i]));
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

    private void OnIsDriveChanged(object    sender,
                                  EventArgs e) {
        var modifiedPost = (PostInfo)sender;

        if (modifiedPost.IsDrive) { // 由普通立柱变成驱动立柱
            if (!TrackerModel!.HasSlew) return;
            modifiedPost.LeftToBeam  = 75;
            modifiedPost.RightToBeam = 75;
            // 找到上面的主梁，将其分成两段接入
        } else { // 由驱动立柱变成普通立柱
            // 将两侧主梁开断都置为0
            modifiedPost.LeftToBeam  = 0;
            modifiedPost.RightToBeam = 0;
            if (TrackerModel!.HasSlew) { // 如果有点击需要修改驱动两侧的主梁，合并为同一个主梁
                var leftBeam  = modifiedPost.PostModel.PreItem as BeamModel;
                var rightBeam = modifiedPost.PostModel.NextItem as BeamModel;
                modifiedPost.PostModel.PreItem  = null;
                modifiedPost.PostModel.NextItem = null;
                if (leftBeam != null) {
                    leftBeam.EndX        = modifiedPost.X - TrackerModel!.BeamGap / 2;
                    leftBeam.Length      = leftBeam.EndX  - leftBeam.StartX;
                    leftBeam.RightToNext = TrackerModel!.BeamGap;
                    leftBeam.NextItem    = rightBeam ?? null;
                }

                if (rightBeam != null) {
                    rightBeam.StartX    = modifiedPost.X + TrackerModel!.BeamGap / 2;
                    rightBeam.Length    = rightBeam.EndX - rightBeam.StartX;
                    rightBeam.LeftToPre = TrackerModel!.BeamGap;
                    rightBeam.PreItem   = leftBeam ?? null;
                }
            }
        }

        TrackerModel!.UpdatePostX();
        UpdatePostInfos();
    }

    #endregion

    #region 主梁事件，主梁只有长度变化影响绘图，暂不考虑

    //当主梁分段长度发生变化时更新主梁分段长度，不对数组进行增减操作
    private void OnBeamLengthChanged(object    sender,
                                     EventArgs e) {
        MessageBox.Show("主梁长度发生改变");
        if (BeamInfos == null) {
            return;
        }

        var currentBeam = (BeamInfo)sender;
        if (currentBeam.Length < TrackerModel!.MinBeamLength) {
            MessageBox.Show("主梁长度不可超过" + TrackerModel.MinBeamLength.ToString(CultureInfo.InvariantCulture));
        }

        currentBeam.Length = TrackerModel!.MinBeamLength;
        UpdateBeamInfos();
    }

    #endregion


    // 当TrackerModel属性发生改变时也要重新绘图，主要是末端余量的变化
    private void OnTrackerModelChanged(object                   sender,
                                       PropertyChangedEventArgs e) {
        TrackerModel!.InitPost();
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
        var maxY = TrackerModel!.BeamCenterToGround + TrackerModel.BeamHeight * (1 + TrackerModel.BeamRadio) +
                   TrackerModel.PurlinHeight        + TrackerModel.ModuleHeight;
        var maxX = Math.Max(Math.Max(TrackerModel!.SystemLength, PostInfos == null ? 0 : PostInfos.Last().X),
                            (BeamInfos == null || BeamInfos.Count == 0) ? 0 : BeamInfos.Last().EndX);

        const double scaleRadioX = 0.95;
        const double scaleRadioY = 0.95;

        var scaleX = maxX == 0 ? 0 : scaleRadioX * CanvasWidth  / maxX;
        var scaleY = maxY == 0 ? 0 : scaleRadioY * CanvasHeight / maxY;

        var startX = CanvasWidth * (1 - scaleRadioX) / 2;


        // 画立柱部分
        if (PostInfos != null) {
            PostLines = [];
            foreach (var postInfo in PostInfos) {
                PostLines.Add(new CanvasPostLine(postInfo, (postInfo.X     + TrackerModel.LeftRemind) * scaleX + startX,
                                                 CanvasHeight, (postInfo.X + TrackerModel.LeftRemind) * scaleX + startX,
                                                 CanvasHeight - TrackerModel.BeamCenterToGround * scaleY));
            }
        }

        // 画主梁部分
        if (BeamInfos != null) {
            BeamLines = [];
            var randomColors = SolidColorBrushGenerator.GenerateSolidColorBrushes(BeamInfos.Count);
            for (var i = 0; i < BeamInfos.Count; i++) {
                BeamLines.Add(new CanvasBeamLine(BeamInfos[i], randomColors[i],
                                                 (BeamInfos[i].StartX + TrackerModel.LeftRemind) * scaleX + startX,
                                                 CanvasHeight - TrackerModel.BeamCenterToGround * scaleY,
                                                 (BeamInfos[i].EndX + TrackerModel.LeftRemind) * scaleX + startX,
                                                 CanvasHeight - TrackerModel.BeamCenterToGround * scaleY));
            }
        }

        // 画组件部分
        if (TrackerModel!.PurlinList != null) {
            ModuleLines = [];
            var moduleY = TrackerModel.BeamCenterToGround + TrackerModel.BeamHeight * (1 + TrackerModel.BeamRadio) +
                          TrackerModel.PurlinHeight;
            for (var i = 0; i < TrackerModel.PurlinList.Count - 1; i++) {
                var leftPurlin  = TrackerModel.PurlinList[i];
                var rightPurlin = TrackerModel.PurlinList[i + 1];
                if (rightPurlin.Type == -1) continue; // 右侧檩条为左檩条跳出循环

                ModuleLines.Add(new
                                    CanvasModuleLine((leftPurlin.X + TrackerModel.ModuleGapAxis / 2 + TrackerModel.LeftRemind) * scaleX + startX,
                                                     CanvasHeight - moduleY * scaleY,
                                                     (rightPurlin.X - TrackerModel.ModuleGapAxis / 2 +
                                                      TrackerModel.LeftRemind) * scaleX + startX,
                                                     CanvasHeight - moduleY * scaleY));
            }
        }
    }

    #endregion
}