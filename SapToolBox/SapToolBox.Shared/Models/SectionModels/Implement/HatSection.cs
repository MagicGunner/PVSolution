using System;
using Prism.Mvvm;
using SapToolBox.Shared.Models.SectionModels.Interface;

namespace SapToolBox.Shared.Models.SectionModels.Implement;

public class HatSection(string? name, double _W, double _H, double _B, double _t, double _r) : BindableBase, ISection {
    public string? Name {
        get => name;
        set => SetProperty(ref name, value);
    }

    public string?             Material { get; set; }

    public double H {
        get => _B;
        set => SetProperty(ref _B, value);
    }

    public double W {
        get => _H;
        set => SetProperty(ref _H, value);
    }

    public double WW { // 几字型钢翻边两端点长度
        get => _W;
        set => SetProperty(ref _W, value);
    }

    public double r {
        get => _r;
        set => SetProperty(ref _r, value);
    }

    public double t {
        get => _t;
        set => SetProperty(ref _t, value);
    }

    public double L => (WW - H) / 2 + t;
    public double R => r            + t / 2; // 中心R角
    public double Alpha = 1;
    public double A => H - (2 * R + t);
    public double B => W - (2 * R + t);
    public double C => Alpha * (L - (R + t / 2));
    public double U => 0.5   * Math.PI * R;

    public double ABar => H - t;
    public double BBar => W - t / 2 - Alpha * t / 2;
    public double CBar => Alpha * (L - t / 2);

    public double Area => t * (A + 2 * B + 2 * U + Alpha * (2 * C + 2 * U));

    public double Xc =>
        2 * t / Area * (B * (B / 2 + R) + U * 0.363 * R + Alpha * (U * (B + 1.637 * R) + C * (B + 2 * R))); // 形心坐标

    public double Yc => 0;

    public double Ixx { get; }
    public double Iyy { get; }
    public double Ixy { get; }
    public double J   { get; }
    public double Wxx { get; }
    public double Wyy { get; }
    public double Zxx { get; }
    public double Zyy { get; }
    public double Rxx { get; }
    public double Ryy { get; }
    public double X0  { get; }
    public double Cw  { get; }


    public void SetEffectiveWidth(double sigmaMax,
                                  double sigmaMin,
                                  double sigma1) {
    }
}