using System;
using Prism.Mvvm;
using SapToolBox.Resource.DesignResources.DesignLookUps;
using SapToolBox.Shared.Models.DesignCodeModels.Interface;
using SapToolBox.Shared.Models.SectionModels.Interface;

namespace SapToolBox.Shared.Models.DesignCodeModels.Implement;

public class ChineseCode(ISection section,
                         double   e,
                         double   fu,
                         double   fy,
                         double   fb,
                         double   fv,
                         double   unbracedLength,
                         double   lengthFactorX,
                         double   lengthFactorY,
                         double   lengthFactorW,
                         bool     hasSwayX,
                         bool     hasSwayY,
                         double   factorAlpha,
                         double   p_1,
                         double   vx_1,
                         double   vy_1,
                         double   mx_1,
                         double   my_1,
                         double   t_1,
                         double   p_2,
                         double   vx_2,
                         double   vy_2,
                         double   mx_2,
                         double   my_2,
                         double   t_2
) : BindableBase, IDesignCode {
#region 构造函数

    // 双向压弯构件构造函数
    public ChineseCode(ISection section,
                       double   e,
                       double   fu,
                       double   fy,
                       double   fb,
                       double   fv,
                       double   unbracedLength,
                       double   lengthFactorX,
                       double   lengthFactorY,
                       double   lengthFactorW,
                       bool     hasSwayX,
                       bool     hasSwayY,
                       double   factorAlpha,
                       double   p,
                       double   mx,
                       double   my) : this(section,
                                           e,
                                           fu,
                                           fy,
                                           fb,
                                           fv,
                                           unbracedLength,
                                           lengthFactorX,
                                           lengthFactorY,
                                           lengthFactorW,
                                           hasSwayX,
                                           hasSwayY,
                                           factorAlpha,
                                           p,
                                           0,
                                           0,
                                           mx,
                                           my,
                                           0,
                                           p,
                                           0,
                                           0,
                                           mx,
                                           my,
                                           0) {
    }

    // 单向压弯构件构造函数
    public ChineseCode(ISection section,
                       double   e,
                       double   fu,
                       double   fy,
                       double   fb,
                       double   fv,
                       double   unbracedLength,
                       double   lengthFactorX,
                       double   lengthFactorY,
                       double   lengthFactorW,
                       bool     hasSwayX,
                       bool     hasSwayY,
                       double   factorAlpha,
                       double   p,
                       double   m) : this(section,
                                          e,
                                          fu,
                                          fy,
                                          fb,
                                          fv,
                                          unbracedLength,
                                          lengthFactorX,
                                          lengthFactorY,
                                          lengthFactorW,
                                          hasSwayX,
                                          hasSwayY,
                                          factorAlpha,
                                          p,
                                          0,
                                          0,
                                          m,
                                          0,
                                          0,
                                          p,
                                          0,
                                          0,
                                          m,
                                          0,
                                          0) {
    }

#endregion


#region 和外力无关的相关属性

    public ISection Section {
        get => section;
        set => SetProperty(ref section, value);
    }

    public double E {
        get => e;
        set {
            if (!SetProperty(ref e, value)) return;
            UpdateGeneralProperties();
        }
    }

    public double Fu {
        get => fu;
        set {
            if (!SetProperty(ref fu, value)) return;
            UpdateGeneralProperties();
        }
    }

    public double Fy {
        get => fy;
        set {
            if (!SetProperty(ref fy, value)) return;
            UpdateGeneralProperties();
        }
    }

    public double Fb {
        get => fb;
        set {
            if (!SetProperty(ref fb, value)) return;
            UpdateGeneralProperties();
        }
    }

    public double Fv {
        get => fv;
        set {
            if (!SetProperty(ref fv, value)) return;
            UpdateGeneralProperties();
        }
    }

    public double UnbracedLength {
        get => unbracedLength;
        set {
            if (!SetProperty(ref unbracedLength, value)) return;
            UpdateGeneralProperties();
        }
    }

    public double LengthFactorX {
        get => lengthFactorX;
        set {
            if (!SetProperty(ref lengthFactorX, value)) return;
            UpdateGeneralProperties();
        }
    }

    public double LengthFactorY {
        get => lengthFactorY;
        set {
            if (!SetProperty(ref lengthFactorY, value)) return;
            UpdateGeneralProperties();
        }
    }

    // 扭转屈曲相关参数，与缀板相关
    public double FactorAlpha {
        get => factorAlpha;
        set {
            if (!SetProperty(ref factorAlpha, value)) return;
            UpdateGeneralProperties();
        }
    }

    public double LengthFactorW {
        get => lengthFactorW;
        set {
            if (!SetProperty(ref lengthFactorW, value)) return;
            UpdateGeneralProperties();
        }
    }

    public bool HasSwayX {
        get => hasSwayX;
        set {
            if (!SetProperty(ref hasSwayX, value)) return;
            UpdateGeneralProperties();
        }
    }

    public bool HasSwayY {
        get => hasSwayY;
        set {
            if (!SetProperty(ref hasSwayY, value)) return;
            UpdateGeneralProperties();
        }
    }

    public double EffectiveLengthX => UnbracedLength * LengthFactorX;

    public double EffectiveLengthY => UnbracedLength * LengthFactorY;

    public double EffectiveLengthW => UnbracedLength * LengthFactorW;

    public double LambdaX => EffectiveLengthX / Section.Rxx;

    public double LambdaY => EffectiveLengthY / Section.Ryy;

    public double LambdaW {
        get {
            if (EffectiveLengthW == 0) return 0;
            var ss = LambdaX
                   * LambdaX
                   / Section.Area
                   * (Section.Cw / EffectiveLengthW / EffectiveLengthW + 0.039 * Section.J);
            var i0Square = Section.X0 * Section.X0 + Section.Rxx * Section.Rxx + Section.Ryy * Section.Ryy;
            return LambdaX
                 * Math.Sqrt((ss + i0Square) / 2 / ss
                           + Math.Sqrt(Math.Pow((ss + i0Square) / 2          / ss, 2)
                                     - (i0Square - FactorAlpha  * Section.X0 * Section.X0) / ss));
        }
    }

    public double Lambda => Math.Max(LambdaW, Math.Max(LambdaX, LambdaY));

    public double Epsilon => Math.Sqrt(235 / Fy);

    public double LambdaModified => Lambda / Epsilon;

    public double LambdaXModified => LambdaX / Epsilon;

    public double LambdaYModified => LambdaX / Epsilon;

    public double Phi => ChineseDesignLookUp.Instance.GetChineseColdFormedPhi(LambdaModified);

    public double PhiX => ChineseDesignLookUp.Instance.GetChineseColdFormedPhi(LambdaXModified);

    public double PhiY => ChineseDesignLookUp.Instance.GetChineseColdFormedPhi(LambdaYModified);

#endregion

#region 和外力有关的相关属性

    // 节点1受力
    public double P_1 {
        get => p_1;
        set {
            if (!SetProperty(ref p_1, value)) return;
            UpdateDesignProperties();
        }
    }

    public double Vx_1 {
        get => vx_1;
        set {
            if (!SetProperty(ref vx_1, value)) return;
            UpdateDesignProperties();
        }
    }

    public double Vy_1 {
        get => vy_1;
        set {
            if (!SetProperty(ref vy_1, value)) return;
            UpdateDesignProperties();
        }
    }

    public double Mx_1 {
        get => mx_1;
        set {
            if (!SetProperty(ref mx_1, value)) return;
            UpdateDesignProperties();
        }
    }

    public double My_1 {
        get => my_1;
        set {
            if (!SetProperty(ref my_1, value)) return;
            UpdateDesignProperties();
        }
    }

    public double T_1 {
        get => t_1;
        set {
            if (!SetProperty(ref t_1, value)) return;
            UpdateDesignProperties();
        }
    }

    // 节点2受力
    public double P_2 {
        get => p_2;
        set {
            if (!SetProperty(ref p_2, value)) return;
            UpdateDesignProperties();
        }
    }

    public double Vx_2 {
        get => vx_2;
        set {
            if (!SetProperty(ref vx_2, value)) return;
            UpdateDesignProperties();
        }
    }

    public double Vy_2 {
        get => vy_2;
        set {
            if (!SetProperty(ref vy_2, value)) return;
            UpdateDesignProperties();
        }
    }

    public double Mx_2 {
        get => mx_2;
        set {
            if (!SetProperty(ref mx_2, value)) return;
            UpdateDesignProperties();
        }
    }

    public double My_2 {
        get => my_2;
        set {
            if (!SetProperty(ref my_2, value)) return;
            UpdateDesignProperties();
        }
    }

    public double T_2 {
        get => t_2;
        set {
            if (!SetProperty(ref t_2, value)) return;
            UpdateDesignProperties();
        }
    }

    // 计算是把弯矩认为是单调的
    public double SigmaMax1 => Section.Wxx == 0 ? 0 : P_1 * 1e3 / Section.Area + Mx_1 * 1e6 / Section.Wxx;
    public double SigmaMin1 => Section.Wxx == 0 ? 0 : P_1 * 1e3 / Section.Area - Mx_1 * 1e6 / Section.Wxx;

    public double SigmaMax2 => Section.Wxx == 0 ? 0 : P_2 * 1e3 / Section.Area + Mx_2 * 1e6 / Section.Wxx;
    public double SigmaMin2 => Section.Wxx == 0 ? 0 : P_2 * 1e3 / Section.Area - Mx_2 * 1e6 / Section.Wxx;

    public int MaxPoint =>
        Math.Max(Math.Abs(SigmaMax1), Math.Abs(SigmaMin1)) > Math.Max(Math.Abs(SigmaMax2), Math.Abs(SigmaMin2)) ? 1 : 2;

    public double P        => MaxPoint == 1 ? P_1 : P_2;
    public double MMajor   => MaxPoint == 1 ? Mx_1 : Mx_2;
    public double MMinor   => MaxPoint == 1 ? My_1 : My_2;
    public double SigmaMax => MaxPoint == 1 ? SigmaMax1 : SigmaMax2;
    public double SigmaMin => MaxPoint == 1 ? SigmaMin1 : SigmaMin2;

#endregion


#region 通用方法区

    public void UpdateGeneralProperties() {
        RaisePropertyChanged(nameof(EffectiveLengthX));
        RaisePropertyChanged(nameof(EffectiveLengthY));
        RaisePropertyChanged(nameof(EffectiveLengthW));
        RaisePropertyChanged(nameof(LambdaX));
        RaisePropertyChanged(nameof(LambdaY));
        RaisePropertyChanged(nameof(LambdaW));
        RaisePropertyChanged(nameof(Lambda));
        RaisePropertyChanged(nameof(Epsilon));
        RaisePropertyChanged(nameof(LambdaModified));
        RaisePropertyChanged(nameof(Phi));
        RaisePropertyChanged(nameof(PhiX));
        RaisePropertyChanged(nameof(PhiY));
    }

    public void UpdateDesignProperties() {
        RaisePropertyChanged(nameof(SigmaMax1));
        RaisePropertyChanged(nameof(SigmaMin1));
        RaisePropertyChanged(nameof(SigmaMax2));
        RaisePropertyChanged(nameof(SigmaMin2));
        RaisePropertyChanged(nameof(MaxPoint));
        RaisePropertyChanged(nameof(P));
        RaisePropertyChanged(nameof(MMajor));
        RaisePropertyChanged(nameof(MMinor));
        RaisePropertyChanged(nameof(SigmaMax));
        RaisePropertyChanged(nameof(SigmaMin));

        SetSectionEffectiveWidth();
    }

    public void SetSectionEffectiveWidth() {
        switch (P) {
            case > 0 when MMajor == 0 && MMinor == 0: // 轴心受压构件
                Section.SetEffectiveWidth(SigmaMax, SigmaMin, Fb * Math.Min(PhiX, PhiY));
                break;
            case > 0 when MMajor != 0: // 压弯构件
                Section.SetEffectiveWidth(SigmaMax, SigmaMin, Fb);
                break;
            case < 0 when MMajor != 0: // 拉弯构件
                Section.SetEffectiveWidth(SigmaMax, SigmaMin, Fb * Math.Min(PhiX, PhiY));
                break;
        }
    }

#endregion
}