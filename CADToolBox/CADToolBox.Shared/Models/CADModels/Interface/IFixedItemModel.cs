namespace CADToolBox.Shared.Models.CADModels.Interface;

public interface IFixedItemModel {
    public string? SectionType { get; set; } // 主梁截面类型
    public string? Section     { get; set; } // 主梁截面
    public string? Material    { get; set; } // 主梁材质
}