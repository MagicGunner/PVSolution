using CADToolBox.Resource.NameDictionarys;
using CADToolBox.Shared.Models.CADModels.Interface;
using System.Collections.Generic;

namespace CADToolBox.Shared.Models.CADModels.Implement;

public class PostModel : IItemModel {
    public IItemModel? PreItem  { get; set; }
    public IItemModel? NextItem { get; set; }

    public int     Num             { get; set; } // 立柱序号
    public bool    IsDrive         { get; set; } // 是否驱动
    public bool    IsMotor         { get; set; } // 是否回转
    public string? SectionType     { get; set; } // 立柱截面类型
    public string? Section         { get; set; } // 立柱截面
    public string? Material        { get; set; } // 立柱材质
    public double  PileUpGround    { get; set; } // 基础露头
    public double  PileDownGround  { get; set; } // 基础埋深
    public double  FlangeThickness { get; set; } // 法兰板厚度
    public double  LeftToBeam      { get; set; } // 立柱左侧开断
    public double  RightToBeam     { get; set; } // 立柱右侧开断
    public double  PileWidth       { get; set; } // 基础宽度
    public double  LeftSpan        { get; set; } // 左侧跨距
    public double  RightSpan       { get; set; } // 右侧跨距

    public double X      { get; set; } // 立柱相对于组件最左侧坐标
    public double StartX { get; set; }
    public double EndX   { get; set; }
}