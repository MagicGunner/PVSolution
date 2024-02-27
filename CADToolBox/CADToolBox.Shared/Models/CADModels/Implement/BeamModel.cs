namespace CADToolBox.Shared.Models.CADModels.Implement;

public class BeamModel {
    public int     Num         { get; set; } // 主梁序号
    public string? SectionType { get; set; } // 立柱截面类型
    public string? Section     { get; set; } // 立柱截面
    public string? Material    { get; set; } // 立柱材质
    public double  LeftToPre   { get; set; } // 基础露头
    public double  RightToNext { get; set; } // 基础埋深
    public double  Length      { get; set; } // 立柱左侧开断
}