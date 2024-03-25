namespace CADToolBox.Shared.Models.CADModels.Interface;

public interface IFixedItemModel {
    public string? SectionType { get; set; } // 截面类型
    public string? Section     { get; set; } // 截面
    public string? Material    { get; set; } // 材质
    public double  Length      { get; set; } // 长度
    public double  Thickness   { get; set; } // 厚度
}