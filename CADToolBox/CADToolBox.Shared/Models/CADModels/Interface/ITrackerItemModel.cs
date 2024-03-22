namespace CADToolBox.Shared.Models.CADModels.Interface;

public interface ITrackerItemModel {
    public double StartX { get; set; }
    public double EndX   { get; set; }

    public ITrackerItemModel? PreItem  { get; set; }
    public ITrackerItemModel? NextItem { get; set; }

    public string? SectionType { get; set; } // 主梁截面类型
    public string? Section     { get; set; } // 主梁截面
    public string? Material    { get; set; } // 主梁材质
}