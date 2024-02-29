
namespace CADToolBox.Shared.Models.UIModels.Interface;

public interface ITrackerItemInfo {
    public int Num { get; set; } // 序号
    public bool IsDetailsVisible { get; set; } // 细节是否展示
    public bool IsSelected       { get; set; } // 是否被选中
}