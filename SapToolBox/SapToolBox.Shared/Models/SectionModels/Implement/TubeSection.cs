using System;
using Prism.Mvvm;
using SapToolBox.Shared.Models.SectionModels.Interface;

namespace SapToolBox.Shared.Models.SectionModels.Implement;

public class TubeSection(string? name, double _H, double _B, double _t, double _r) : BindableBase, ISection {
    public string? Name {
        get => name;
        set => SetProperty(ref name, value);
    }

    public double H {
        get => _H;
        set => SetProperty(ref _H, value);
    }

    public double B {
        get => _B;
        set => SetProperty(ref _B, value);
    }

    public double t {
        get => _t;
        set => SetProperty(ref _t, value);
    }

    public double r {
        get => _r;
        set => SetProperty(ref _r, value);
    }

    public string? Material { get; set; }
    public double  Area     { get; }
    public double  Ixx      { get; }
    public double  Iyy      { get; }
    public double  Ixy      { get; }
    public double  J        { get; }
    public double  Wxx      { get; }
    public double  Wyy      { get; }
    public double  Zxx      { get; }
    public double  Zyy      { get; }
    public double  Rxx      { get; }
    public double  Ryy      { get; }
    public double  X0       { get; }
    public double  Cw       { get; }


    public void SetEffectiveWidth(double sigmaMax,
                                  double sigmaMin,
                                  double sigma1) {
    }
}