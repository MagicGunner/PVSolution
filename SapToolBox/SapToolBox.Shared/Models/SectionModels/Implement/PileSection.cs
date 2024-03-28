using System;
using Prism.Mvvm;
using SapToolBox.Shared.Models.SectionModels.Interface;

namespace SapToolBox.Shared.Models.SectionModels.Implement;

public class PileSection(string? name, double outerDia, double thickness) : BindableBase, ISection {
    public double D {
        get => outerDia;
        set => SetProperty(ref outerDia, value);
    }

    public double t {
        get => thickness;
        set => SetProperty(ref thickness, value);
    }


    public string? Name {
        get => name;
        set => SetProperty(ref name, value);
    }

    public string?             Material { get; set; }
    public double              Area     { get; }
    public double              Ixx      { get; }
    public double              Iyy      { get; }
    public double              Ixy      { get; }
    public double              J        { get; }
    public double              Wxx      { get; }
    public double              Wyy      { get; }
    public double              Zxx      { get; }
    public double              Zyy      { get; }
    public double              Rxx      { get; }
    public double              Ryy      { get; }
    public double              X0       { get; }
    public double              Cw       { get; }

    public void SetEffectiveWidth(double sigmaMax,
                                  double sigmaMin,
                                  double sigma1) {
    }
}