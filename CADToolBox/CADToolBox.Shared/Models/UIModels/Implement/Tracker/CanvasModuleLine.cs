using CADToolBox.Shared.Models.CADModels.Implement;
using CADToolBox.Shared.Models.UIModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.UIModels.Implement.Tracker;

public class CanvasModuleLine(
    double x1,
    double y1,
    double x2,
    double y2
) : ObservableObject {
    public double X1 { get; set; } = x1;
    public double Y1 { get; set; } = y1;
    public double X2 { get; set; } = x2;
    public double Y2 { get; set; } = y2;
}