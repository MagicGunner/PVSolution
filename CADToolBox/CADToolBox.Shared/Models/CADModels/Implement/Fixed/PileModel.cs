using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.CADModels.Implement.Fixed;

public partial class PileModel : ObservableObject {
    #region 字段属性

    [ObservableProperty]
    private string? _type; // 基础类型

    [ObservableProperty]
    private string? _section; //基础截面

    #endregion
}