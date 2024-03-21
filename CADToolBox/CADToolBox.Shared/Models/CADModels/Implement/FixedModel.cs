using CADToolBox.Shared.Models.CADModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.CADModels.Implement;

public class FixedModel : ObservableObject, IPvSupport {
    public string? ProjectName    { get; set; }
    public double  ModuleLength   { get; set; }
    public double  ModuleWidth    { get; set; }
    public double  ModuleHeight   { get; set; }
    public double  ModuleGapChord { get; set; }
    public double  ModuleGapAxis  { get; set; }
    public double  MinGroundDist  { get; set; }
    public double  PileUpGround   { get; set; }
    public double  PileWidth      { get; set; }
}