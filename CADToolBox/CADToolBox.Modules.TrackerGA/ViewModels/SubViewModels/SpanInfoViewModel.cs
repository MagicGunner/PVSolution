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
                                               };
                newPostInfo.RightSpanChanged += (obj,
                                                 _) => {
                                                    var modifiedPost = (PostInfo)obj;
                                                    if (modifiedPost.Num == PostInfos.Count) return;
                                                    PostInfos![modifiedPost.Num].LeftSpan = modifiedPost.RightSpan;
                                                };
                newPostInfo.IsDriveChanged += (obj,
                                               _) => {
                                                  var modifiedPost = (PostInfo)obj;
                                                  if (modifiedPost.IsDrive && TrackerModel!.HasSlew) {
                                                      MessageBox.Show("当前立柱由普通立柱变为回转驱动立柱，请注意填写电机处的开断，此处将两侧均置为75");
                                                      modifiedPost.LeftToBeam  = 75;
                                                      modifiedPost.RightToBeam = 75;
                                                  } else {
                                                      modifiedPost.LeftToBeam  = 0;
                                                      modifiedPost.RightToBeam = 0;
                                                  }
                                              };
                newPostInfo.ModelChanged += (_,
                                             _) => {
                                                TrackerModel!.UpdatePost();
                                                UpdateSystemDraw();
                                            };
                PostInfos.Add(newPostInfo);
            }
        }

        BeamInfos = [];
        // 初始化主梁信息
        if (TrackerModel?.BeamList != null) {
            foreach (var newBeamInfo in TrackerModel.BeamList.Select(beamModel => new BeamInfo(beamModel))) {
                newBeamInfo.LengthChanged += (_,
                                              _) => {
                                                 TrackerModel!.UpdateBeam();
                                                 UpdateBeamInfos();
                                                 UpdateSystemDraw();
                                             };
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

        TrackerModel!.UpdatePost();
        UpdatePostInfos();
    }

    [RelayCommand]
    private void DeletePost(PostInfo currentPostInfo) {
        if (PostInfos!.Count <= 1) {
            MessageBox.Show("至少需要一个立柱");
        }

        TrackerModel!.PostList!.RemoveAt(currentPostInfo.Num - 1);
        TrackerModel!.UpdatePost();
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
        var currentBeam = currentBeamInfo.BeamModel!;
        var insertBeam  = BeamInfoMapper.Map<BeamModel, BeamModel>(currentBeam);

        if (currentBeamInfo.Num == BeamInfos!.Count) {
            TrackerModel!.BeamList!.Insert(BeamInfos!.Count - 1, insertBeam);
        } else {
            TrackerModel!.BeamList!.Insert(currentBeamInfo.Num, insertBeam);
        }

        // 整理主梁坐标再插入到BeamInfos
        TrackerModel!.UpdateBeam();

        UpdateBeamInfos();
    }

    [RelayCommand]
    private void DeleteBeam(BeamInfo currentBeamInfo) {
        if (BeamInfos!.Count <= 1) {
            MessageBox.Show("主梁数量最少为1");
            return;
        }

        TrackerModel!.BeamList!.RemoveAt(currentBeamInfo.Num - 1);
        TrackerModel!.UpdateBeam();

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

        // 实际主梁列表与主梁信息列表相同的部分
        var count = Math.Min(TrackerModel!.BeamList.Count, BeamInfos.Count);
        for (var i = 0; i < count; i++) {
            BeamInfos[i].BeamModel = TrackerModel.BeamList[i];
        }

        if (TrackerModel!.BeamList.Count >= BeamInfos.Count) {
            // 展示的比实际的少需要增加
            for (var i = count; i < TrackerModel!.BeamList.Count; i++) {
                BeamInfos.Add(new BeamInfo(TrackerModel!.BeamList[i]));
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

        var count = Math.Min(TrackerModel!.PostList.Count, PostInfos.Count);
        for (var i = 0; i < count; i++) {
            PostInfos[i].PostModel = TrackerModel.PostList[i];
        }

        if (TrackerModel!.PostList.Count >= PostInfos.Count) {
            for (int i = count; i < TrackerModel!.PostList.Count; i++) {
                PostInfos.Add(new PostInfo(TrackerModel!.PostList[i]));
            }
        } else {
            for (var i = PostInfos.Count - 1; i >= count; i--) {
                PostInfos.RemoveAt(i);
            }
        }
    }

    #endregion

    #region 处理事件

    #region 立柱事件

    //处理立柱展示数组发生变化
    //private void OnPostInfosListChanged(object               sender,
    //                                    ListChangedEventArgs e) {
    //    switch (e.ListChangedType) {
    //        case ListChangedType.ItemAdded:   break;
    //        case ListChangedType.ItemDeleted: break;
    //        case ListChangedType.Reset:       break;
    //        case ListChangedType.ItemMoved:   break;
    //        case ListChangedType.ItemChanged:
    //            var changedIndex   = e.NewIndex;
    //            var changedItem    = PostInfos?[changedIndex];
    //            var propDescriptor = e.PropertyDescriptor;
    //            if (propDescriptor != null) {
    //                switch (propDescriptor.Name) {
    //                    case nameof(changedItem.LeftSpan):
    //                        OnPostLeftSpanChanged(changedItem!);
    //                        UpdateSystemDraw();
    //                        break;
    //                    case nameof(changedItem.RightSpan):
    //                        OnPostRightSpanChanged(changedItem!);
    //                        UpdateSystemDraw();
    //                        break;
    //                    case nameof(changedItem.IsDrive):
    //                        OnPostIsDriveChanged(changedItem!);
    //                        UpdateSystemDraw();
    //                        break;
    //                }
    //            }

    //            break;
    //        case ListChangedType.PropertyDescriptorAdded:   break;
    //        case ListChangedType.PropertyDescriptorDeleted: break;
    //        case ListChangedType.PropertyDescriptorChanged: break;
    //        default:                                        throw new ArgumentOutOfRangeException();
    //    }
    //}

    // 立柱左跨距发生改变
    //private void OnPostLeftSpanChanged(PostInfo postInfo) {
    //    if (postInfo.Num == 1) return; // 改变的为第一个立柱时无需更新
    //    PostInfos![postInfo.Num - 2].RightSpan = postInfo.LeftSpan;
    //}

    // 立柱右跨距发生改变
    //private void OnPostRightSpanChanged(PostInfo postInfo) {
    //    if (postInfo.Num == PostInfos!.Count) return; // 改变的为最后一个立柱时无需更新
    //    PostInfos![postInfo.Num].LeftSpan = postInfo.RightSpan;
    //}

    // 增加立柱
    //private void OnPostAdd(PostInfo postInfo) {
    //    if (PostInfos?.Count == 0) return;
    //    for (var i = 0; i < PostInfos!.Count; i++) {
    //        PostInfos[i].Num = i + 1;
    //    }

    //    OnPostLeftSpanChanged(postInfo);
    //    OnPostRightSpanChanged(postInfo);
    //}

    // 减少立柱
    //private void OnPostDelete(PostInfo  oldPostInfo,
    //                          PostInfo? newPostInfo) {
    //    if (PostInfos?.Count == 0) return;
    //    for (var i = 0; i < PostInfos!.Count; i++) {
    //        PostInfos[i].Num = i + 1;
    //    }

    //    if (newPostInfo != null) {
    //        OnPostLeftSpanChanged(newPostInfo);
    //        OnPostRightSpanChanged(newPostInfo);
    //    }

    //    UpdateSystemDraw();
    //}

    // 立柱类型发生改变
    //private void OnPostIsDriveChanged(PostInfo postInfo) {
    //    if (TrackerModel!.DriveGap == 0) return; // 如果没有驱动间隙不影响立柱位置
    //}

    #endregion

    #region 主梁事件，主梁只有长度变化影响绘图，暂不考虑

    // 处理主梁展示数组发生变化
    //private void OnBeamInfosListChanged(object               sender,
    //                                    ListChangedEventArgs e) {
    //    switch (e.ListChangedType) {
    //        case ListChangedType.ItemAdded:   break;
    //        case ListChangedType.ItemDeleted: break;
    //        case ListChangedType.Reset:       break;
    //        case ListChangedType.ItemMoved:   break;
    //        case ListChangedType.ItemChanged:
    //            //var changedIndex   = e.NewIndex;
    //            //var changedItem    = BeamInfos![changedIndex];
    //            //var propDescriptor = e.PropertyDescriptor;
    //            //if (propDescriptor != null) {
    //            //    switch (propDescriptor.Name) {
    //            //        case nameof(BeamInfo.Length):
    //            //            TrackerModel!.UpdateBeam();
    //            //            UpdateBeamInfos();
    //            //            break;
    //            //    }
    //            //}

    //            break;
    //        case ListChangedType.PropertyDescriptorAdded:   break;
    //        case ListChangedType.PropertyDescriptorDeleted: break;
    //        case ListChangedType.PropertyDescriptorChanged: break;
    //        default:                                        throw new ArgumentOutOfRangeException();
    //    }
    //}


    // 主梁数量发生改变
    //private void OnBeamNumChanged() {
    //    if (BeamInfos!.Count <= 0) return;
    //    for (var i = 0; i < BeamInfos!.Count; i++) {
    //        BeamInfos[i].Num = i + 1;
    //    }

    //    OnBeamLengthChanged();
    //}

    //当主梁分段长度发生变化时更新主梁分段长度，不对数组进行增减操作
    //private void OnBeamLengthChanged() {
    //    if (BeamInfos == null) {
    //        return;
    //    }

    //    BeamInfos.DisableEvents();
    //    var totalLength = TrackerModel!.SystemLength;
    //    BeamInfos[0].Length =  Math.Min(totalLength, BeamInfos[0].Length);
    //    totalLength         -= BeamInfos[0].Length;
    //    for (var i = 1; i < BeamInfos!.Count; i++) {
    //        if (totalLength <= 0) {
    //            BeamInfos[i].Length = 0;
    //        } else {
    //            BeamInfos[i].Length =  Math.Min(totalLength, BeamInfos[i].Length);
    //            totalLength         -= BeamInfos[i].Length + BeamInfos[i].LeftToPre;
    //        }
    //    }

    //    BeamInfos.EnableEvents();
    //    UpdateSystemDraw();
    //}

    #endregion


    // 当TrackerModel属性发生改变时也要重新绘图，主要是末端余量的变化
    private void OnTrackerModelChanged(object                   sender,
                                       PropertyChangedEventArgs e) {
        TrackerModel!.UpdatePost();
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