using System;
using SapToolBox.Shared.Models.SectionModels.Interface;

namespace SapToolBox.Shared.Models.SectionModels.Implement;

public class LSection : ISection {
    public string?             Name     { get; set; }
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
    public event EventHandler? PropertyChanged;

    public void SetEffectiveWidth(double sigmaMax,
                                  double sigmaMin,
                                  double sigma1) {
        throw new NotImplementedException();
    }
}