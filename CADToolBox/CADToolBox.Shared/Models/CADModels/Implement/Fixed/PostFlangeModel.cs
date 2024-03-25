using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.CADModels.Implement.Fixed;

public partial class PostFlangeModel : ObservableObject {
    #region 字段属性

    [ObservableProperty]
    private double _thickness; // 法兰板厚度

    #endregion
}