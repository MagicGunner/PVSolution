using CADToolBox.Shared.Models.CADModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.CADModels.Implement.Fixed;

public partial class PurlinModel : ObservableObject {
    #region 通用属性

    [ObservableProperty]
    private string? _sectionType;

    [ObservableProperty]
    private string? _section;

    [ObservableProperty]
    private string? _material;

    [ObservableProperty]
    private double _length;

    [ObservableProperty]
    private double _thickness;

    #endregion
}