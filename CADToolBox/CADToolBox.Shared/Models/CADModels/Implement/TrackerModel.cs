using CADToolBox.Shared.Models.CADModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.CADModels.Implement;

public class TrackerModel : ObservableObject, IPvSupport {
#region 通用属性

    public string? ProjectName { get; set; }

    public double ModuleLength   { get; set; }
    public double ModuleWidth    { get; set; }
    public double ModuleHeight   { get; set; }
    public double ModuleGapChord { get; set; }
    public double ModuleGapAxis  { get; set; }
    public double MinGroundDist  { get; set; }
    public double PileUpGround   { get; set; }
    public double PileWidth      { get; set; }

#endregion

#region 跟踪支架属性

    public double PurlinHeight     { get; set; } // 檩条高度
    public double PurlinLengh      { get; set; } // 檩条长度
    public double BeamHeight       { get; set; } // 主梁高度
    public double BeamWidth        { get; set; } // 主梁宽度
    public double ModuleRowCounter { get; set; } // 组件排数
    public double StowAngle        { get; set; } // 保护角度
    public double MaxAngle         { get; set; } // 最大角度
    public double BeamCenterToDp   { get; set; } // 旋转中心到驱动立柱顶部距离
    public double BeamCenterToGp   { get; set; } // 旋转中性到普通立柱顶部距离
    public double BeamRadio        { get; set; } // 主梁上下部分比值
    public double PostWidth        { get; set; } // 立柱宽度

#endregion

#region 构造函数

    public TrackerModel(double moduleLength) {
        ModuleLength = moduleLength;
    }

#endregion
}