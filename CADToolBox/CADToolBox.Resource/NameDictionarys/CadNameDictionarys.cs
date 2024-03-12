using System.Collections.Generic;

namespace CADToolBox.Resource.NameDictionarys;

public static class CadNameDictionarys {
    public static readonly Dictionary<string, string> AttrNameDic;

    static CadNameDictionarys() {
        AttrNameDic = new Dictionary<string, string> {
                                                         { "ProjectName", "项目名称" },
                                                         { "ModuleLength", "组件长" },
                                                         { "ModuleWidth", "组件宽" },
                                                         { "ModuleHeight", "组件高" },
                                                         { "ModuleGapChord", "组件间隙(弦长方向)" },
                                                         { "ModuleGapAxis", "组件间隙(主轴方向)" },
                                                         { "MinGroundDist", "最小离地高度" },
                                                         { "StowAngle", "保护角度" },
                                                         { "MaxAngle", "最大角度" },
                                                         { "PurlinHeight", "檩条高度" },
                                                         { "PurlinWidth", "檩条宽度" },
                                                         { "BeamHeight", "主梁高度" },
                                                         { "BeamWidth", "主梁宽度" },
                                                         { "PurlinLength", "檩条长度" },
                                                         { "ModuleRowCounter", "单套组件排数" },
                                                         { "ModuleColCounter", "单套组件列数" },
                                                         { "DriveGap", "驱动间隙" },
                                                         { "BeamGap", "主梁间隙" },
                                                         { "BeamRadio", "主梁上下比值" },
                                                         { "BeamCenterToDrivePost", "旋转中心至驱动立柱顶面距离" },
                                                         { "BeamCenterToGeneralPost", "旋转中心至普通立柱顶面距离" },
                                                         { "LeftRemind", "左侧末端余量" },
                                                         { "RightRemind", "右侧末端余量" },
                                                         { "PostWidth", "立柱截面高度" },
                                                         { "PileUpGround", "基础露出地面" },
                                                         { "PileWidth", "基础截面高度" }
                                                     };
    }
}