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
        WeakReferenceMessenger.Default.Register<WindowSizeChangedMessage>(this,
                                                                          (obj,
                                                                           message) => {
                                                                              CanvasHeight = message.CanvasHeight;
                                                                              CanvasWidth  = message.CanvasWidth;
                                                                              // 画图
                                                                              TrackerModel!.SortPostX();
                                                                              TrackerModel!.SortBeamX();
                                                                              Draw();
                                                                          });
        PostInfos    = [];
        BeamInfos    = [];
        TrackerModel = TrackerApp.Current.TrackerModel;
        if (TrackerModel?.PostList == null) return;
        // 初始化立柱信息
        foreach (var postModel in TrackerModel.PostList) {
            PostInfos.Add(new PostInfo(postModel));
            PostInfos.Last().SpanChanged    += OnPostSpanChanged;
            PostInfos.Last().IsDriveChanged += OnIsDriveChanged;
        }

        PostInfos.CollectionChanged += OnPostInfosListChanged;
        // 初始化主梁信息
        if (TrackerModel?.BeamList == null) return;
        foreach (var beamModel in TrackerModel.BeamList) {
            BeamInfos.Add(new BeamInfo(beamModel));
            BeamInfos.Last().LengthChanged += OnBeamLengthChanged;
        }


        TrackerModel.InitPurlin(); // 后续要考虑换位置**************************************


        BeamInfos.CollectionChanged += OnBeamInfosListChanged;
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
        TrackerModel!.PostList!.RemoveAt(currentPostInfo.Num - 1);
        PostInfos?.Remove(currentPostInfo);
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
        var insertBeam = BeamInfoMapper.Map<BeamModel, BeamModel>(currentBeamInfo.BeamModel);
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
            foreach (var postInfo in PostInfos) { postInfo.IsSelected = false; }

        foreach (var selectedItem in selectedItems) {
            if (selectedItem is PostInfo selectedPost) selectedPost.IsSelected = true;
        }

        Draw();
    }

    // 前台选中的主梁发生改变时
    [RelayCommand]
    private void BeamInfoSelectionChanged(IList selectedItems) {
        if (BeamInfos != null)
            foreach (var beamInfo in BeamInfos) { beamInfo.IsSelected = false; }

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
        postInfo.SpanChanged += OnPostSpanChanged;
        PostInfos?.Insert(index, postInfo);
        postInfo.OnSpanChanged();
    }

    private void InsertBeamInfo(int       index,
                                BeamModel beamModel) {
        TrackerModel!.BeamList!.Insert(index, beamModel);
        var beamInfo = new BeamInfo(beamModel);
        beamInfo.LengthChanged += OnBeamLengthChanged;
        BeamInfos?.Insert(index, beamInfo);
    }


    private void SortPostNum() {
        if (PostInfos!.Count <= 0) return;
        for (var i = 0; i    < PostInfos!.Count; i++) { PostInfos[i].Num = i + 1; }
    }

    private void SortBeamNum() {
        if (BeamInfos!.Count <= 0) return;
        for (var i = 0; i    < BeamInfos!.Count; i++) { BeamInfos[i].Num = i + 1; }
    }


    // 当立柱跨距发生变化时出发
    private void OnPostSpanChanged(object    sender,
                                   EventArgs e) {
        if (PostInfos!.Count <= 1) return; // 只有一个立柱时无需更新跨距
        var currentPostInfo   = (PostInfo)sender;
        var modifiedPostIndex = currentPostInfo.Num - 1;
        // 更新前一个立柱的右跨距
        if (modifiedPostIndex != 0) { // 当改变的立柱为第一个时无需操作
            PostInfos[modifiedPostIndex - 1].SpanChanged -= OnPostSpanChanged;
            PostInfos[modifiedPostIndex - 1].RightSpan   =  currentPostInfo.LeftSpan;
            PostInfos[modifiedPostIndex - 1].SpanChanged += OnPostSpanChanged;
        }

        // 更新后一个立柱的左跨距
        if (modifiedPostIndex == PostInfos.Count - 1) return; // 当改变的立柱为最后一根立柱时当前操作无需进行
        PostInfos[modifiedPostIndex + 1].SpanChanged -= OnPostSpanChanged;
        PostInfos[modifiedPostIndex + 1].LeftSpan    =  currentPostInfo.RightSpan;
        PostInfos[modifiedPostIndex + 1].SpanChanged += OnPostSpanChanged;

        // 跨距发生改变需要更新坐标与图形
        TrackerModel!.OnPostListChanged();
        Draw();
    }

    // 当有立柱修改立柱类型时触发
    private void OnIsDriveChanged(object    sender,
                                  EventArgs e) {
        TrackerModel!.OnPostListChanged();
        Draw();
        MessageBox.Show("立柱类型发生改变");
    }

    // 当主梁分段长度发生变化时触发
    private void OnBeamLengthChanged(object    sender,
                                     EventArgs e) {
        TrackerModel!.SortBeamX();
        Draw();
    }

    //处理立柱数量发生变化
    private void OnPostInfosListChanged(object                           sender,
                                        NotifyCollectionChangedEventArgs e) {
        MessageBox.Show("立柱数量发生变化");
        SortPostNum();
        // 立柱数量发生改变需要更新图形
        TrackerModel!.OnPostListChanged(); // 更新驱动立柱数量等

        Draw();
    }

    // 处理主梁发生变化
    private void OnBeamInfosListChanged(object                           sender,
                                        NotifyCollectionChangedEventArgs e) {
        SortBeamNum();
        TrackerModel!.SortBeamX();
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
        var maxY = TrackerModel!.BeamCenterToGround
                 + TrackerModel.BeamHeight * (1 + TrackerModel.BeamRadio)
                 + TrackerModel.PurlinHeight
                 + TrackerModel.ModuleHeight;
        var maxX   = Math.Max(TrackerModel!.SystemLength, PostInfos == null ? 0 : PostInfos.Last().X);
        var scaleX = maxX == 0 ? 0 : 0.95 * CanvasWidth / maxX;
        var scaleY = maxY == 0
                         ? 0
                         : 0.95
                         * CanvasHeight
                         / maxY;


        // 画立柱部分
        if (PostInfos != null) {
            PostLines = [];
            foreach (var postInfo in PostInfos) {
                PostLines.Add(new CanvasPostLine(postInfo,
                                                 (postInfo.X + TrackerModel.LeftRemind) * scaleX,
                                                 CanvasHeight,
                                                 (postInfo.X + TrackerModel.LeftRemind) * scaleX,
                                                 CanvasHeight - TrackerModel.BeamCenterToGround * scaleY));
            }
        }

        // 画主梁部分
        if (BeamInfos != null) {
            BeamLines = [];
            var randomColors = SolidColorBrushGenerator.GenerateSolidColorBrushes(7);
            for (var i = 0; i < BeamInfos.Count; i++) {
                BeamLines.Add(new CanvasBeamLine(BeamInfos[i],
                                                 randomColors[i],
                                                 (BeamInfos[i].StartX + TrackerModel.LeftRemind) * scaleX,
                                                 CanvasHeight - TrackerModel.BeamCenterToGround * scaleY,
                                                 (BeamInfos[i].EndX + TrackerModel.LeftRemind) * scaleX,
                                                 CanvasHeight - TrackerModel.BeamCenterToGround * scaleY));
            }
        }

        // 画组件部分
        if (TrackerModel!.PurlinList != null) {
            ModuleLines = [];
            var moduleY = TrackerModel.BeamCenterToGround
                        + TrackerModel.BeamHeight * (1 + TrackerModel.BeamRadio)
                        + TrackerModel.PurlinHeight;
            for (var i = 0; i < TrackerModel.PurlinList.Count - 1; i++) {
                var leftPurlin  = TrackerModel.PurlinList[i];
                var rightPurlin = TrackerModel.PurlinList[i + 1];
                if (rightPurlin.Type == -1) continue; // 右侧檩条为左檩条跳出循环

                ModuleLines.Add(new CanvasModuleLine((leftPurlin.X + TrackerModel.ModuleGapAxis / 2) * scaleX,
                                                     CanvasHeight - moduleY * scaleY,
                                                     (rightPurlin.X - TrackerModel.ModuleGapAxis / 2) * scaleX,
                                                     CanvasHeight - moduleY * scaleY));
            }
        }
    }

#endregion
}