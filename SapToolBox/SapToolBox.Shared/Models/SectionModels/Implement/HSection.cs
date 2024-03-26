using System;
using Prism.Mvvm;
using SapToolBox.Shared.Models.SectionModels.Interface;

namespace SapToolBox.Shared.Models.SectionModels.Implement;

public class HSection(string name, double h, double b, double tf, double tw, double r) : BindableBase, ISection {
    public double H {
        get => h;
        set => UpdateProperties(ref h, value);
    } // 总高度

    public double B {
        get => b;
        set => UpdateProperties(ref b, value);
    } // 总宽度

    public double Tf {
        get => tf;
        set => UpdateProperties(ref tf, value);
    } // 翼板厚度

    public double Tw {
        get => tw;
        set => UpdateProperties(ref tw, value);
    } // 腹板厚度

    public double R {
        get => r;
        set => UpdateProperties(ref r, value);
    } // R角


    public string Name { get; set; } = name;

    public double Area {
        get {
            var area = Tw * (H - 2 * Tf) + 2 * Tf * B;
            if (R > 0) {
                area += 4 * R * R - Math.PI * R * R;
            }

            return area;
        }
    }

    public double Ixx => B * Math.Pow(H, 3) / 12 - 2 * ((B - Tw) / 2 * Math.Pow((H - 2 * Tf), 3) / 12);

    public double Iyy =>
        H * Math.Pow(B, 3) / 12 - 2 * ((H - 2 * Tf) * Math.Pow((B - Tw) / 2, 3) / 12 +
                                       (H - 2 * Tf) * ((B - Tw) / 2) * Math.Pow((B - Tw) / 4 + Tw / 2, 2));

    public double Ixy { get; set; }
    public double J   { get; set; }
    public double Wxx { get; set; }
    public double Wyy { get; set; }
    public double Zxx { get; set; }
    public double Zyy { get; set; }
    public double Rxx { get; set; }
    public double Ryy { get; set; }
    public double X0  { get; set; }

    public double Cw => throw new NotImplementedException();

    public event EventHandler PropertyChanged;

    public void SetEffectiveWidth(double sigmaMax, double sigmaMin, double sigma1) {
        throw new NotImplementedException();
    }


    public HSection(string name, double h, double b, double tf, double tw) : this(name, h, b, tf, tw, 0) {
    }

    private void UpdateProperties(ref double prop, double value) {
        if (!SetProperty(ref prop, value)) return;
        RaisePropertyChanged(nameof(Ixx));
        RaisePropertyChanged(nameof(Iyy));
        RaisePropertyChanged(nameof(Area));
    }
}