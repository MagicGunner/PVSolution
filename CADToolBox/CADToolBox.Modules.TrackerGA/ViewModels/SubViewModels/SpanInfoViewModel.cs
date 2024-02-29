using System;
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
using AutoMapper;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CADToolBox.Shared.Models.CADModels.Implement;
using CADToolBox.Shared.Models.UIModels.Implement;
using CADToolBox.Shared.Models.UIModels.Interface;

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
    private List<string> _sectionMaterial = [
                                                "Q235",
                                                "Q355",
                                                "Q420",
                                                "Q500"
                                            ];

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
        PostInfos    = [];
        BeamInfos    = [];
        TrackerModel = TrackerApp.Current.TrackerModel;
        if (TrackerModel?.PostList == null)
            return;
        // 初始化立柱信息
        foreach (var postModel in TrackerModel.PostList) {
            PostInfos.Add(new PostInfo(postModel));
            PostInfos.Last().SpanChanged += OnPostSpanChanged;
        }

        PostInfos.CollectionChanged += OnPostInfosListChanged;
        // 初始化主梁信息
        if (TrackerModel?.BeamList == null)
            return;
        foreach (var beamModel in TrackerModel.BeamList) {
            BeamInfos.Add(new BeamInfo(beamModel));
        }

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
        PostInfos?.Remove(currentPostInfo);
        TrackerModel!.PostList!.RemoveAt(currentPostInfo.Num - 1);
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
        BeamInfos?.Remove(currentBeamInfo);
        TrackerModel!.BeamList!.RemoveAt(currentBeamInfo.Num - 1);
    }

#endregion

#region 普通方法区

    //partial void OnFilterTextIndexChanged(int value) {
    //    if (value == 0) // 全不选
    //        PostInfos.ForEach(postInfo => postInfo.IsSelected = false);
    //    else if (value == 1) // 全选
    //        PostInfos.ForEach(postInfo => postInfo.IsSelected = true);
    //    else if (value == 2) { // 选中驱动立柱
    //        PostInfos.ForEach(postInfo => postInfo.IsSelected = postInfo.IsDrive);
    //    } else // 选中普通立柱
    //        PostInfos.ForEach(postInfo => postInfo.IsSelected = !postInfo.IsDrive);
    //}

    private void InsertPostInfo(int       index,
                                PostModel postModel) {
        TrackerModel!.PostList!.Insert(index, postModel);
        var postInfo = new PostInfo(postModel);
        postInfo.SpanChanged += OnPostSpanChanged;
        PostInfos?.Insert(index, postInfo);
    }

    private void InsertBeamInfo(int       index,
                                BeamModel beamModel) {
        TrackerModel!.BeamList!.Insert(index, beamModel);
        var beamInfo = new BeamInfo(beamModel);
        BeamInfos?.Insert(index, beamInfo);
    }

    private void SortPostNum() {
        if (PostInfos!.Count <= 0)
            return;
        for (var i = 0; i < PostInfos!.Count; i++) {
            PostInfos[i].Num = i + 1;
        }
    }

    private void SortBeamNum() {
        if (BeamInfos!.Count <= 0)
            return;
        for (var i = 0; i < BeamInfos!.Count; i++) {
            BeamInfos[i].Num = i + 1;
        }
    }


    private void OnPostSpanChanged(object    sender,
                                   EventArgs e) {
        MessageBox.Show("立柱跨距发生变化");
        if (PostInfos!.Count <= 1)
            return; // 只有一个立柱时无需更新跨距
        var currentPostInfo   = (PostInfo)sender;
        var modifiedPostIndex = currentPostInfo.Num - 1;
        // 更新前一个立柱的右跨距
        if (modifiedPostIndex != 0) { // 当改变的立柱为第一个时无需操作
            PostInfos[modifiedPostIndex - 1].SpanChanged -= OnPostSpanChanged;
            PostInfos[modifiedPostIndex - 1].RightSpan   =  currentPostInfo.LeftSpan;
            PostInfos[modifiedPostIndex - 1].SpanChanged += OnPostSpanChanged;
        }

        // 更新后一个立柱的左跨距
        if (modifiedPostIndex == PostInfos.Count - 1)
            return; // 当改变的立柱为最后一根立柱时当前操作无需进行
        PostInfos[modifiedPostIndex + 1].SpanChanged -= OnPostSpanChanged;
        PostInfos[modifiedPostIndex + 1].LeftSpan    =  currentPostInfo.RightSpan;
        PostInfos[modifiedPostIndex + 1].SpanChanged += OnPostSpanChanged;
    }

    //处理立柱发生变化
    private void OnPostInfosListChanged(object                           sender,
                                        NotifyCollectionChangedEventArgs e) {
        SortPostNum();
    }

    // 处理主梁发生变化
    private void OnBeamInfosListChanged(object                           sender,
                                        NotifyCollectionChangedEventArgs e) {
        SortBeamNum();
    }

#endregion
}