namespace CADToolBox.Shared.Models.CADModels.Implement;

public class BeamModel {
    public int     Num         { get; set; } // 主梁序号
    public string? SectionType { get; set; } // 主梁截面类型
    public string? Section     { get; set; } // 主梁截面
    public string? Material    { get; set; } // 主梁材质
    public double  LeftToPre   { get; set; } // 与上一个主梁距离
    public double  RightToNext { get; set; } // 与下一个主梁距离
    public double  Length      { get; set; } // 主梁长度

    public double StartX { get; set; } // 主梁当前段左侧相对于组件最左侧坐标
    public double EndX   { get; set; } // 主梁当前段右侧相对于组件最左侧坐标
}