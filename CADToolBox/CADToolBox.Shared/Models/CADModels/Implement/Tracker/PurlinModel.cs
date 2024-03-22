namespace CADToolBox.Shared.Models.CADModels.Implement.Tracker;

public class PurlinModel(
    double x,
    int    type
) {
    public double X    { get; set; } = x;    // 檩条相对组件最左侧的坐标
    public int    Type { get; set; } = type; // 檩条的类型，-1代表左檩条，0代表中檩条，1代表右檩条
}