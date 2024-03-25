using System.Collections.ObjectModel;
using CADToolBox.Shared.Models.CADModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.CADModels.Implement.Fixed;

public partial class PostModel : ObservableObject {
    #region 通用属性

    [ObservableProperty]
    private string? _sectionType;

    [ObservableProperty]
    private string? _section;

    [ObservableProperty]
    private double _length;

    [ObservableProperty]
    private string? _material;

    [ObservableProperty]
    private double _thickness;

    #endregion

    #region 对象属性

    public PostFlangeModel?                  Flange    { get; set; } // 法兰板
    public ObservableCollection<HoopModel>?  HoopList  { get; set; } // 抱箍列表
    public ObservableCollection<BraceModel>? BraceList { get; set; } //斜撑列表

    #endregion
}