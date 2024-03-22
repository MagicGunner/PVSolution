namespace CADToolBox.Shared.Models.CADModels.Interface;

public interface IPvSupport {
#region 设计输入

    public string? ProjectName { get; set; } // 项目名称

    public double ModuleLength   { get; set; } // 组件长
    public double ModuleWidth    { get; set; } // 组件宽
    public double ModuleHeight   { get; set; } // 组件高
    public double ModuleGapChord { get; set; } // 组件间隙(弦长方向)
    public double ModuleGapAxis  { get; set; } // 组件间隙(主轴或檩条方向)

    public double MinGroundDist { get; set; } // 最小离地高度

#endregion


    public double PileUpGround   { get; set; } // 基础露头
    public double PileDownGround { get; set; } // 基础埋深

    public double PileWidth    { get; set; } // 基础宽度

    public int ModuleRowCounter { get; set; } // 组件排数
    public int ModuleColCounter { get; set; } // 组件列数
}