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

namespace CADToolBox.Modules.TrackerGA.ViewModels.SubViewModels;

public partial class SpanInfoViewModel : ViewModelBase {
    #region 字段与属性

    [ObservableProperty]
    private TrackerModel? _trackerModel;

    [ObservableProperty]
    private MyBindingList<PostInfo>? _postInfos;

    [ObservableProperty]
    private MyBindingList<BeamInfo>? _beamInfos;

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
                                                                              TrackerModel!.SortPost();
                                                                              TrackerModel!.SortBeam();
                                                                              Draw();
                                                                          });
        PostInfos                    =  [];
        BeamInfos                    =  [];
        TrackerModel                 =  TrackerApp.Current.TrackerModel!;
        TrackerModel.PropertyChanged += OnTrackerModelChanged;
        if (TrackerModel?.PostList == null) return;
        // 初始化立柱信息
        foreach (var postModel in TrackerModel.PostList) {
            PostInfos.Add(new PostInfo(postModel));
        }

        PostInfos.ListChanged += OnPostInfosListChanged;
        // 初始化主梁信息
        if (TrackerModel?.BeamList == null) return;
        foreach (var beamModel in TrackerModel.BeamList) {
            BeamInfos.Add(new BeamInfo(beamModel));
        }

        BeamInfos.ListChanged += OnBeamInfosListChanged;
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
    private void AddPostAtLast() {
        var insertPostModel = PostInfoMapper.Map<PostModel, PostModel>(DefaultPostModel);
        InsertPostInfo(PostInfos!.Count, insertPostModel);
    }

    [RelayCommand]
    private void AddPost(PostInfo currentPostInfo) {
        var insertPost = PostInfoMapper.Map<PostModel, PostModel>(currentPostInfo.PostModel);
        InsertPostInfo(currentPostInfo.Num, insertPost);
    }

    [RelayCommand]
    private void DeletePost(PostInfo currentPostInfo) {
        var oldPostInfo = currentPostInfo;
        var newPostInfo = PostInfos!.Count == 1 ? null : PostInfos[currentPostInfo.Num];
        TrackerModel!.PostList!.RemoveAt(oldPostInfo.Num - 1);
        PostInfos!.Remove(oldPostInfo);
        OnPostDelete(oldPostInfo, newPostInfo);
    }

    #endregion

    #region 操作主梁表格的Relaycommand

    [RelayCommand]
    private void AddBeamAtLast() {
        var insertBeamModel = BeamInfoMapper.Map<BeamModel, BeamModel>(DefaultBeamModel);
        InsertBeamInfo(BeamInfos!.Count, insertBeamModel);
    }

    [RelayCommand]
    private void AddBeam(BeamInfo currentBeamInfo) {
        // 增加主梁时需要判断最右侧主梁是否超过系统总长
        var insertBeam = BeamInfoMapper.Map<BeamModel, BeamModel>(currentBeamInfo.BeamModel);
        var lastBeam   = TrackerModel!.BeamList?.Last();
        if (lastBeam != null && lastBeam.EndX >= TrackerModel.SystemLength - TrackerModel.LeftRemind) {
            // 如果当前主梁已经满了则不可添加
            MessageBox.Show("当前主梁已不可再添加");
            return;
        }

        InsertBeamInfo(currentBeamInfo.Num, insertBeam);
    }

    [RelayCommand]
    private void DeleteBeam(BeamInfo currentBeamInfo) {
        TrackerModel!.BeamList!.RemoveAt(currentBeamInfo.Num - 1);
        BeamInfos?.Remove(currentBeamInfo);
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

        Draw();
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

        Draw();
    }

    #endregion

    #region 普通方法区

    private void InsertPostInfo(int       index,
                                PostModel postModel) {
        TrackerModel!.PostList!.Insert(index, postModel);
        var postInfo = new PostInfo(postModel);
        PostInfos?.Insert(index, postInfo);
    }

    private void InsertBeamInfo(int       index,
                                BeamModel beamModel) {
        TrackerModel!.BeamList!.Insert(index, beamModel);
        var beamInfo = new BeamInfo(beamModel);
        BeamInfos?.Insert(index, beamInfo);
    }

    #endregion

    #region 处理事件

    #region 立柱事件

    //处理立柱展示数组发生变化
    private void OnPostInfosListChanged(object               sender,
                                        ListChangedEventArgs e) {
        switch (e.ListChangedType) {
            case ListChangedType.ItemAdded: // 增加立柱
                var addedIndex = e.NewIndex;
                var addedItem  = PostInfos![addedIndex];
                OnPostAdd(addedItem);
                Draw();
                break;
            case ListChangedType.ItemDeleted: // 减少立柱
                break;
            case ListChangedType.Reset:
                break;
            case ListChangedType.ItemMoved:
                break;
            case ListChangedType.ItemChanged:
                var changedIndex   = e.NewIndex;
                var changedItem    = PostInfos?[changedIndex];
                var propDescriptor = e.PropertyDescriptor;
                if (propDescriptor != null) {
                    switch (propDescriptor.Name) {
                        case nameof(changedItem.LeftSpan):
                            OnPostLeftSpanChanged(changedItem!);
                            Draw();
                            break;
                        case nameof(changedItem.RightSpan):
                            OnPostRightSpanChanged(changedItem!);
                            Draw();
                            break;
                        case nameof(changedItem.IsDrive):
                            OnPostIsDriveChanged(changedItem!);
                            Draw();
                            break;
                    }
                }

                break;
            case ListChangedType.PropertyDescriptorAdded:
                break;
            case ListChangedType.PropertyDescriptorDeleted:
                break;
            case ListChangedType.PropertyDescriptorChanged:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // 立柱左跨距发生改变
    private void OnPostLeftSpanChanged(PostInfo postInfo) {
        if (postInfo.Num == 1)
            return; // 改变的为第一个立柱时无需更新
        PostInfos![postInfo.Num - 2].RightSpan = postInfo.LeftSpan;
    }

    // 立柱右跨距发生改变
    private void OnPostRightSpanChanged(PostInfo postInfo) {
        if (postInfo.Num == PostInfos!.Count)
            return; // 改变的为最后一个立柱时无需更新
        PostInfos![postInfo.Num].LeftSpan = postInfo.RightSpan;
    }

    // 增加立柱
    private void OnPostAdd(PostInfo postInfo) {
        if (PostInfos?.Count == 0)
            return;
        for (var i = 0; i < PostInfos!.Count; i++) {
            PostInfos[i].Num = i + 1;
        }

        OnPostLeftSpanChanged(postInfo);
        OnPostRightSpanChanged(postInfo);
    }

    // 减少立柱
    private void OnPostDelete(PostInfo  oldPostInfo,
                              PostInfo? newPostInfo) {
        if (PostInfos?.Count == 0)
            return;
        for (var i = 0; i < PostInfos!.Count; i++) {
            PostInfos[i].Num = i + 1;
        }

        if (newPostInfo != null) {
            OnPostLeftSpanChanged(newPostInfo);
            OnPostRightSpanChanged(newPostInfo);
        }

        Draw();
    }

    // 立柱类型发生改变
    private void OnPostIsDriveChanged(PostInfo postInfo) {
        if (TrackerModel!.DriveGap == 0)
            return; // 如果没有驱动间隙不影响立柱位置
    }

    #endregion

    #region 主梁事件

    // 处理主梁展示数组发生变化
    private void OnBeamInfosListChanged(object               sender,
                                        ListChangedEventArgs e) {
        switch (e.ListChangedType) {
            case ListChangedType.ItemAdded:
                OnBeamAdd();
                Draw();
                break;
            case ListChangedType.ItemDeleted:
                OnBeamDelete();
                Draw();
                break;
            case ListChangedType.Reset:
                break;
            case ListChangedType.ItemMoved:
                break;
            case ListChangedType.ItemChanged:
                var changedIndex   = e.NewIndex;
                var changedItem    = BeamInfos![changedIndex];
                var propDescriptor = e.PropertyDescriptor;
                if (propDescriptor != null) {
                    switch (propDescriptor.Name) {
                        case nameof(BeamInfo.Length):
                            OnBeamLengthChanged();
                            Draw();
                            break;
                    }
                }

                break;
            case ListChangedType.PropertyDescriptorAdded:
                break;
            case ListChangedType.PropertyDescriptorDeleted:
                break;
            case ListChangedType.PropertyDescriptorChanged:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    // 主梁数量发生改变
    private void OnBeamNumChanged() {
        if (BeamInfos!.Count <= 0)
            return;
        for (var i = 0; i < BeamInfos!.Count; i++) {
            BeamInfos[i].Num = i + 1;
        }
    }

    // 整理主梁分段长度，返回第一个长度为0的主梁序号
    private void ModifyBeamLength() {
        var totalLength = TrackerModel!.SystemLength;
        for (var i = 0; i < BeamInfos!.Count; i++) {
            BeamInfos[i].Length =  Math.Min(totalLength, BeamInfos[i].Length);
            totalLength         -= BeamInfos[i].Length + BeamInfos[i].RightToNext;
        }
    }


    private void OnBeamAdd() {
        OnBeamNumChanged();
        ModifyBeamLength();
    }

    private void OnBeamDelete() {
        OnBeamNumChanged(); // 先更新序号
        ModifyBeamLength();
    }

    // 当主梁分段长度发生变化时更新主梁分段长度
    private void OnBeamLengthChanged() {
        ModifyBeamLength();
    }

    #endregion


    // 当TrackerModel属性发生改变时也要重新绘图，主要是末端余量的变化
    private void OnTrackerModelChanged(object                   sender,
                                       PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(TrackerModel.LeftRemind):
            case nameof(TrackerModel.RightRemind):
                if (BeamInfos == null) {
                    return;
                }

                BeamInfos.Last().Length = TrackerModel!.SystemLength - BeamInfos.Last().StartX;
                break;
            case nameof(TrackerModel.BeamGap):
                if (BeamInfos == null) return;
                foreach (var beamInfo in BeamInfos) {
                    beamInfo.LeftToPre   = TrackerModel!.BeamGap;
                    beamInfo.RightToNext = TrackerModel.BeamGap;
                }

                ModifyBeamLength();

                break;
        }

        Draw();
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


    private void Draw() {
        TrackerModel!.SortPost(); // 每次绘图前都需要整理立柱位置信息
        TrackerModel!.SortBeam(); // 每次绘图前都需要整理主梁位置信息

        var maxY = TrackerModel!.BeamCenterToGround + TrackerModel.BeamHeight * (1 + TrackerModel.BeamRadio) +
                   TrackerModel.PurlinHeight        + TrackerModel.ModuleHeight;
        var maxX = Math.Max(Math.Max(TrackerModel!.SystemLength, PostInfos == null ? 0 : PostInfos.Last().X),
                            BeamInfos == null ? 0 : BeamInfos.Last().EndX);

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