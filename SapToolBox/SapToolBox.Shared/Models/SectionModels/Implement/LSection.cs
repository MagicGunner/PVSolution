using System;
using Prism.Mvvm;
using SapToolBox.Shared.Models.SectionModels.Interface;

namespace SapToolBox.Shared.Models.SectionModels.Implement;

public class LSection(string? name, double _B, double _b, double _t, double _r) : BindableBase, ISection {
    public double B {
        get => _B;
        set => SetProperty(ref _B, value);
    }

    public double b {
        get => _b;
        set => SetProperty(ref _b, value);
    }

    public double t {
        get => _t;
        set => SetProperty(ref _t, value);
    }

    public double r {
        get => _r;
        set => SetProperty(ref _r, value);
    }

    public string? Name {
        get => name;
        set => SetProperty(ref name, value);
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

    public LSection(string? name,
                    double  _b,
                    double  _t,
                    double  _r) : this(name, _b, _b, _t, _r) {
    }

    public void SetEffectiveWidth(double sigmaMax,
                                  double sigmaMin,
                                  double sigma1) {
    }
}