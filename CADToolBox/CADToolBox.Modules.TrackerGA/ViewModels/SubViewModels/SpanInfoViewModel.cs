﻿using System;
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

namespace CADToolBox.Modules.TrackerGA.ViewModels.SubViewModels;

public partial class SpanInfoViewModel : ViewModelBase {
    [ObservableProperty]
    private TrackerModel? _trackerModel;

    [ObservableProperty]
    private ObservableCollection<PostInfo>? _postInfos;
    //public ObservableCollection<>

    //public int CurrentPostNum => PostInfos?.Count;

    [ObservableProperty]
    private PostModel _defaultPostModel = new() {
                                                    Num         = 1,
                                                    IsDrive     = false,
                                                    SectionType = "W型钢",
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


    private readonly MapperConfiguration _postModelConfig = new(cfg => cfg.CreateMap<PostModel, PostModel>());
    private          IMapper             PostInfoMapper => _postModelConfig.CreateMapper();

    // 点击时是选取一行函数单个单元格
    [ObservableProperty]
    private DataGridSelectionUnit _currentSelectionUnit = DataGridSelectionUnit.CellOrRowHeader;

    public SpanInfoViewModel() {
        PostInfos    = [];
        TrackerModel = TrackerApp.Current.TrackerModel;
        // 初始化立柱信息
        if (TrackerModel?.PostList == null) return;
        foreach (var postModel in TrackerModel.PostList) {
            PostInfos.Add(new PostInfo(postModel));
            PostInfos.Last().SpanChanged += OnPostSpanChanged;
        }

        PostInfos.CollectionChanged += (_, e) => { OnPostInfosListChanged(e); };
    }


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

    [RelayCommand]
    private void SwitchDetailVisible(PostInfo currentPostInfo) {
        currentPostInfo.IsDetailsVisible = !currentPostInfo.IsDetailsVisible;
    }

    private void InsertPostInfo(int index, PostModel postModel) {
        TrackerModel!.PostList!.Insert(index, postModel);
        var postInfo = new PostInfo(postModel);
        postInfo.SpanChanged += OnPostSpanChanged;
        PostInfos?.Insert(index, postInfo);
    }

    private void SortPostNum() {
        if (PostInfos!.Count <= 0) return;
        for (var i = 0; i < PostInfos!.Count; i++) {
            PostInfos[i].Num = i + 1;
        }

        MessageBox.Show("立柱数量发生变化");
    }

    private void OnPostSpanChanged(object sender, EventArgs e) {
        MessageBox.Show("立柱跨距发生变化");
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
    }

    //处理立柱发生变化
    private void OnPostInfosListChanged(NotifyCollectionChangedEventArgs e) {
        SortPostNum();
        //switch (e.ListChangedType) {
        //    case ListChangedType.ItemAdded:
        //    case ListChangedType.ItemDeleted:
        //        //CurrentPostNum = PostInfos!.Count;
        //        SortPostNum();
        //        break;
        //    case ListChangedType.ItemChanged: {
        //        var changedIndex       = e.NewIndex;
        //        var changedItem        = PostInfos?[changedIndex];
        //        var propertyDescriptor = e.PropertyDescriptor;

        //        if (propertyDescriptor != null) {
        //            var propertyName = propertyDescriptor.Name;
        //            switch (propertyName) {
        //                case nameof(changedItem.LeftSpan):
        //                    if (changedIndex > 0)
        //                        if (PostInfos != null)
        //                            if (changedItem != null)
        //                                PostInfos[changedIndex - 1].RightSpan = changedItem.LeftSpan;
        //                    break;
        //                case nameof(changedItem.RightSpan):
        //                    if (PostInfos != null && changedIndex < PostInfos.Count - 1)
        //                        if (changedItem != null)
        //                            PostInfos[changedIndex + 1].LeftSpan = changedItem.RightSpan;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //        break;
        //    }
        //    case ListChangedType.Reset:
        //        break;
        //    case ListChangedType.ItemMoved:
        //        break;
        //    case ListChangedType.PropertyDescriptorAdded:
        //        break;
        //    case ListChangedType.PropertyDescriptorDeleted:
        //        break;
        //    case ListChangedType.PropertyDescriptorChanged:
        //        break;
        //    default:
        //        throw new ArgumentOutOfRangeException();
        //}
    }
}