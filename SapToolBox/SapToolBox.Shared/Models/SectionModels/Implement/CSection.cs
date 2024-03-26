using System;
using System.Collections.Generic;
using Prism.Mvvm;
using SapToolBox.Resource.DesignResources.DesignLookUps;
using SapToolBox.Shared.Models.SectionModels.Interface;

namespace SapToolBox.Shared.Models.SectionModels.Implement;

public class CSection(string name, double h, double w, double l, double t, double r) : BindableBase, ISection {
#region 截面参数，和材料属性和外力均无关

    public double H {
        get => h;
        set {
            if (!SetProperty(ref h, value)) return;
            UpdateGeneralProperties();
        }
    }

    public double W {
        get => w;
        set {
            if (!SetProperty(ref w, value)) return;
            UpdateGeneralProperties();
        }
    }

    public double L {
        get => l;
        set {
            if (!SetProperty(ref l, value)) return;
            RaisePropertyChanged(nameof(Alpha));
            UpdateGeneralProperties();
        }
    }

    public double T {
        get => t;
        set {
            if (!SetProperty(ref t, value)) return;
            RaisePropertyChanged(nameof(R));
            UpdateGeneralProperties();
        }
    }

    // 内R角
    public double Rin {
        get => r;
        set {
            if (!SetProperty(ref r, value)) return;
            RaisePropertyChanged(nameof(R));
            RaisePropertyChanged(nameof(U));
            UpdateGeneralProperties();
        }
    }

    public double R     => Rin + T / 2;
    public double Alpha => L == 0 ? 0 : 1;
    public double A     => H - (2 * R + T);
    public double B     => W - (2 * R + T);
    public double C     => Alpha * (L - (R + T / 2));
    public double U     => 0.5   * Math.PI * R;

    public double ABar => H - T;
    public double BBar => W - T / 2 - Alpha * T / 2;
    public double CBar => Alpha * (L - T / 2);

    public double TheoryBlankLength => H + 2 * W + 2 * L - 4 * T;
    public double RealBlankLength   => H + 2 * W + 2 * L - 8 * T;


    public string Name { get; } = name;

    public double Area => T * (A + 2 * B + 2 * U + Alpha * (2 * C + 2 * U));

    public double Ixx =>
        2 * T * (0.0417 * Math.Pow(A, 3) + B * Math.Pow(A / 2 + R, 2) + U * Math.Pow(A / 2 + 0.637 * R, 2) +
                 0.149  * Math.Pow(R, 3) + Alpha * (0.0833 * Math.Pow(C, 3) + C / 4 * Math.Pow(A - C, 2) +
                                                    U      * Math.Pow(A / 2 + 0.637 * R, 2) + 0.149 * Math.Pow(R, 3)));

    public double Xc =>
        2 * T / Area * (B * (B / 2 + R) + U * 0.363 * R + Alpha * (U * (B + 1.637 * R) + C * (B + 2 * R)));

    public double Yc => 0;

    public double Iyy =>
        2 * T * (B * Math.Pow(B / 2 + R, 2) + Math.Pow(B, 3) / 12 + 0.356 * Math.Pow(R, 3) + Alpha *
                 (C * Math.Pow(B + 2 * R, 2) + U * Math.Pow(B + 1.637 * R, 2) + 0.149 * Math.Pow(R, 3))) -
        Area * Math.Pow(Xc, 2);

    public double M =>
        -BBar * ((3 * ABar * ABar * BBar + Alpha * CBar * (6 * ABar * ABar - 8 * CBar * CBar)) /
                 (Math.Pow(ABar, 3) + 6 * ABar * ABar * BBar +
                  Alpha                 * CBar * (8 * CBar * CBar - 12 * ABar * CBar + 6 * ABar * ABar)));

    public double Ixy { get; }

    public double J => Math.Pow(T, 3) / 3 * (A + 2 * B + 2 * U + Alpha * (2 * C + 2 * U));

    public double Wxx => Ixx / (H / 2);

    public double Wyy => Iyy / Math.Max(Xc + T / 2, W - Xc - T / 2);

    public double Zxx => Wxx;

    public double Zyy => Wyy;

    public double Rxx => Math.Sqrt(Ixx / Area);

    public double Ryy => Math.Sqrt(Iyy / Area);

    public double X0 => Xc - M;


    public double Cw =>
        Math.Pow(ABar * BBar, 2) * T / 12 * ((2 * Math.Pow(ABar, 3) * BBar + 3 * Math.Pow(ABar * BBar, 2) + Alpha *
                                              (48 * Math.Pow(CBar, 4) + 112 * BBar * Math.Pow(CBar, 3) +
                                               8  * ABar * Math.Pow(CBar, 3) + 48 * ABar * BBar * CBar * CBar +
                                               12 * ABar * ABar * CBar * CBar + 12 * ABar * ABar * BBar * CBar +
                                               6  * Math.Pow(ABar, 3) * CBar)) /
                                             (6 * Math.Pow(ABar, 2) * BBar + Math.Pow(ABar + Alpha * 2 * CBar, 3) -
                                              Alpha * 24 * ABar * Math.Pow(CBar, 2)));

    public double BetaW => -(T * Xc * Math.Pow(ABar, 3) / 12 + T * Math.Pow(Xc, 3) * ABar);

    public double BetaF =>
        T               / 2 * (Math.Pow(BBar - Xc, 4)    - Math.Pow(Xc, 4)) +
        T * ABar * ABar / 4 * ((BBar - Xc) * (BBar - Xc) - Xc * Xc);

    public double BetaT =>
        Alpha * (2         * CBar        * T * Math.Pow(BBar - Xc, 3) +
                 2 * T / 3 * (BBar - Xc) * (Math.Pow(ABar / 2, 3) - Math.Pow(ABar / 2 - CBar, 3)));

    public double JJ => (BetaW + BetaF + BetaT) / 2 / Iyy + X0;

#endregion

#region 有效截面参数，与材料属性和外力有关

    // 腹板有效宽度相关信息
    private Tuple<double, double, double, double> _webEff;

    public Tuple<double, double, double, double> WebEff {
        get => _webEff;
        set {
            if (SetProperty(ref _webEff, value)) {
                UpdateGeneralProperties();
            }
        }
    }

    // 受压侧翼板有效宽度相关信息
    private Tuple<double, double, double, double> _flangeEffMax;

    public Tuple<double, double, double, double> FlangeEffMax {
        get => _flangeEffMax;
        set => SetProperty(ref _flangeEffMax, value);
    }

    // 受拉侧翼板有效宽度相关信息
    private Tuple<double, double, double, double> _flangeEffMin;

    public Tuple<double, double, double, double> FlangeEffMin {
        get => _flangeEffMin;
        set => SetProperty(ref _flangeEffMin, value);
    }

    public double MinLipLength { get; private set; }

    public double Ae            => WebEff.Item1;
    public double AeCritical    => WebEff.Item4;
    public double BeMax         => FlangeEffMax.Item1;
    public double BeMaxCritical => FlangeEffMax.Item4;
    public double BeMin         => FlangeEffMin.Item1;
    public double BeMinCritical => FlangeEffMin.Item4;

    public double AreaEff => T * (Ae + BeMax + BeMin + 2 * U + Alpha * (2 * C + 2 * U));

    public double YcEff {
        get {
            // 初始形心纵坐标为中心
            // 圆角处和翻边处正负抵消
            var eOfFlange = H / 2 - T / 2;
            // 两个翼板面积距，上侧坐标为正，下侧为负
            var Ay = T * (BeMax - BeMin) * eOfFlange;
            // 上侧腹板有效面积部分
            var webBe1 = WebEff.Item2;
            Ay += webBe1 * T * (H / 2  - Rin - T - webBe1 / 2);
            var webBe2 = Ae            - webBe1;
            Ay += webBe2 * T * (-H / 2 + Rin + T + webBe2 / 2);
            return Ay / AreaEff;
        }
    }

    public double IxxEff {
        get {
            // 初始形心纵坐标为中心
            var eOfFlange = H / 2 - T / 2;
            // 两个翼板 绕自身惯性矩 面积二次矩
            var Ie  = Math.Pow(T, 3) * (BeMax + BeMin) / 12;
            var Ay2 = T                                * (BeMax + BeMin) * eOfFlange * eOfFlange;
            // 腹板有效面积部分
            var webBe1 = WebEff.Item2;
            Ie  += T * Math.Pow(webBe1, 3) / 12;
            Ay2 += webBe1                  * T * Math.Pow(H / 2 - Rin - T - webBe1 / 2, 2);
            var webBe2 = Ae - webBe1;
            Ie  += T * Math.Pow(webBe2, 3) / 12;
            Ay2 += webBe2                  * T * Math.Pow(-H / 2 + Rin + T + webBe2 / 2, 2);
            // 倒角
            Ie  += L == 0 ? 0.149 * Math.Pow(R, 3) * 2 : 0.149 * Math.Pow(R, 3) * 4;
            Ay2 += L == 0 ? U     * T * Math.Pow(0.637 * R + A / 2, 2) * 2 : U * T * Math.Pow(0.637 * R + A / 2, 2) * 4;
            //翻边全部有效
            Ie  += 2 * T * Math.Pow(C, 3) / 12;
            Ay2 += 2                      * T * C * Math.Pow(H / 2 - Rin - T - C / 2, 2);
            return Ie + Ay2;
        }
    }

    public double WxxEff => IxxEff / (Math.Abs(YcEff) + H / 2);

    public List<double> ReduceRatio => [1 - AreaEff / Area, 1 - IxxEff / Ixx, 1 - WxxEff / Wxx];

    /// <summary>
    /// 计算web和flange(受压区)的有效宽度
    /// 该方法由外部的DesignModel调用，执行完计算更新属性
    /// </summary>
    /// <param name="sigmaMax">截面最大压应力</param>
    /// <param name="sigmaMin">截面最小压应力</param>
    /// <param name="sigma1">抗弯强度设计值</param>
    public void SetEffectiveWidth(double sigmaMax, double sigmaMin, double sigma1) {
    #region 考虑相邻板组的约束系数，后面考虑

        //// 腹板有效宽度计算参数
        //var sigmaRadio  = (sigmaMax - sigmaMin) / H; // 应力变化率
        //var webSigmaMax = sigmaMax - sigmaRadio * (Rin + T);
        //var webSigmaMin = sigmaMin + sigmaRadio * (Rin + T);
        //var webPsi      = webSigmaMin / webSigmaMax;
        //webPsi = webPsi < -1 ? -1 : webPsi;
        //var webK = webPsi switch {
        //               > 0 and < 1    => 7.8 - 8.15 * webPsi + 4.35 * webPsi * webPsi,
        //               >= -1 and <= 0 => 7.8 - 6.29 * webPsi + 9.78 * webPsi * webPsi,
        //               _              => throw new ArgumentOutOfRangeException()
        //           };

        //// 受压侧翼板有效宽度计算参数
        //var    flangePsi = 1.0;
        //double flangeK;
        //// 默认最大压应力作用于支承边,此处有疑问需要仔细研究**********************************************************
        //if (l == 0) { // 翻边为0时当前截面为折弯槽钢，翼板视为非加筋板件
        //    flangeK = flangePsi switch {
        //                  // 默认最大压应力作用于支承边,此处有疑问需要仔细研究**********************************************************
        //                  > 0 and <= 1     => 1.70 - 3.025 * flangePsi + 1.75 * flangePsi * flangePsi,
        //                  > -0.4 and <= 0  => 1.70 - 1.75  * flangePsi + 55   * flangePsi * flangePsi,
        //                  > -1 and <= -0.4 => 6.07 - 9.51  * flangePsi + 8.33 * flangePsi * flangePsi,
        //                  _                => throw new ArgumentOutOfRangeException()
        //              };
        //} else {
        //    var b  = B;
        //    var a  = C;
        //    var I  = Math.Pow(a, 3) * T * (1 + 4 * b / a) / 12 / (1 + b / a);
        //    var hw = A;
        //}


        //// 翻边有效宽度计算系数
        //var lipPsi = 1.0;
        //var lipK = flangePsi switch {
        //               // 默认最大压应力作用于支承边,此处有疑问需要仔细研究**********************************************************
        //               > 0 and <= 1     => 1.70 - 3.025 * flangePsi + 1.75 * flangePsi * flangePsi,
        //               > -0.4 and <= 0  => 1.70 - 1.75  * flangePsi + 55   * flangePsi * flangePsi,
        //               > -1 and <= -0.4 => 6.07 - 9.51  * flangePsi + 8.33 * flangePsi * flangePsi,
        //               _                => throw new ArgumentOutOfRangeException()
        //           };

    #endregion

        var sigmaRadio = (sigmaMax - sigmaMin) / H; // 应力变化率

        // 腹板有效宽度计算参数
        var webSigmaMax       = sigmaMax - sigmaRadio * (Rin + T);
        var webSigmaMin       = sigmaMin + sigmaRadio * (Rin + T);
        var webEffWidthResult = GetEffectiveWidth(A, T, webSigmaMax, webSigmaMin, sigma1, 2);
        WebEff = webSigmaMax <= 0 ? Tuple.Create(A, 0.5 * A, 0.5 * A, A) : webEffWidthResult;
        // 受压侧翼板有效宽度计算参数
        var flangeSigmaMax = sigmaMax - sigmaRadio * T / 2;
        var flangeSigmaMin = sigmaMin + sigmaRadio * T / 2;
        if (l > 0) { // 翻边C型钢
            // 最大压应力侧
            FlangeEffMax = flangeSigmaMax < 0
                               ? Tuple.Create(B, 0.5 * B, 0.5 * B, B)
                               : GetEffectiveWidth(B, T, flangeSigmaMax, flangeSigmaMax, sigma1, 1);
            // 最小压应力侧
            FlangeEffMin = flangeSigmaMin < 0
                               ? Tuple.Create(B, 0.5 * B, 0.5 * B, B)
                               : GetEffectiveWidth(B, T, flangeSigmaMin, flangeSigmaMin, sigma1, 1);

            // 翻边只需校核是否满足最小宽厚比要求
        }
        else { // 无翻边C型钢
            // 最大压应力侧
            FlangeEffMax = flangeSigmaMax < 0
                               ? Tuple.Create(B, 0.5 * B, 0.5 * B, B)
                               : GetEffectiveWidth(B, T, flangeSigmaMax, flangeSigmaMax, sigma1, 0);
            // 最小压应力侧
            FlangeEffMin = flangeSigmaMin < 0
                               ? Tuple.Create(B, 0.5 * B, 0.5 * B, B)
                               : GetEffectiveWidth(B, T, flangeSigmaMin, flangeSigmaMin, sigma1, 0);
        }

        MinLipLength = ChineseDesignLookUp.Instance.GetChineseColdFormedLipMinAtRatio(B / T) * T + Rin + T;

        // 通知有效截面参数更新
        //RaisePropertyChanged(nameof(WebEff));
        //RaisePropertyChanged(nameof(FlangeEffMax));
        RaisePropertyChanged(nameof(MinLipLength));
        RaisePropertyChanged(nameof(Ae));
        RaisePropertyChanged(nameof(AeCritical));
        RaisePropertyChanged(nameof(BeMax));
        RaisePropertyChanged(nameof(BeMaxCritical));
        RaisePropertyChanged(nameof(BeMin));
        RaisePropertyChanged(nameof(BeMinCritical));
        RaisePropertyChanged(nameof(AreaEff));
        RaisePropertyChanged(nameof(YcEff));
        RaisePropertyChanged(nameof(IxxEff));
        RaisePropertyChanged(nameof(WxxEff));
        RaisePropertyChanged(nameof(ReduceRatio));
    }


    /// <summary>
    /// 计算有效宽度，不考虑相邻杆件的板组约束系数
    /// </summary>
    /// <param name="flatWidth">平板宽度</param>
    /// <param name="thickness">平板厚度</param>
    /// <param name="sigmaMax">最大压应力</param>
    /// <param name="sigmaMin">最小压应力</param>
    /// <param name="sigma1">受压板件边缘的最大控制力</param> 
    /// <param name="elementType">板件类型，0为非加筋板件，1为部分加筋板件，2为加筋板件</param>
    /// <returns>返回有效宽度和临界宽度(低于这个宽度无需折减)</returns>
    public Tuple<double, double, double, double> GetEffectiveWidth(
        double flatWidth,
        double thickness,
        double sigmaMax,
        double sigmaMin,
        double sigma1,
        int    elementType) {
        if (thickness == 0) return Tuple.Create(0.0, 0.0, 0.0, 0.0);

        var b_tRadio = flatWidth / thickness;                        // 宽厚比
        var psi      = sigmaMin  / sigmaMax;                         // // 应力分布不均匀系数
        var alpha    = psi < 0 ? 1.15 : 1.15 - 0.15;                 // 计算系数
        var bc       = psi >= 0 ? flatWidth : flatWidth / (1 - psi); // 板件受压区宽度
        psi = psi < -1 ? -1 : psi;                                   // 当psi小于1时按照1.0考虑

        // 暂时不考虑板组之间的约束系数
        const double k1 = 1.0;
        var k = elementType switch {
                    0 => psi switch { // 最大压应力作用在支承边
                             > 0 and <= 1     => 1.70 - 3.025 * psi + 1.75 * psi * psi,
                             > -0.4 and <= 0  => 1.70 - 1.75  * psi + 55   * psi * psi,
                             > -1 and <= -0.4 => 6.07 - 9.51  * psi + 8.33 * psi * psi,
                             _                => throw new ArgumentOutOfRangeException()
                         },
                    1 => 5.89 - 11.59 * psi + 6.68 * psi * psi, // 最大压应力作用在支承边
                    2 => psi switch {
                             > 0 and <= 1   => 7.8 - 8.15 * psi + 4.35 * psi * psi,
                             >= -1 and <= 0 => 7.8 - 6.29 * psi + 9.78 * psi * psi,
                             _              => throw new ArgumentOutOfRangeException()
                         },
                    _ => throw new ArgumentOutOfRangeException()
                };


        var pho = Math.Sqrt(205.0 * k1 * k / sigma1);

        var critical1 = 18 * alpha * pho;
        var critical2 = 38 * alpha * pho;

        var criticalWidth = // 小于该值时截面全部有效
            critical1 * thickness;
        var be = bc;
        if (b_tRadio > critical1 && b_tRadio <= critical2) {
            be = bc * (Math.Sqrt(21.8 * alpha * pho / b_tRadio) - 0.1);
        }
        else if (b_tRadio >= critical2) {
            be = bc * 25 * alpha * pho / b_tRadio;
        }

        var be1 = elementType == 2 && psi >= 0 ? 2 * be / (5 - psi) : 0.4 * be;
        var be2 = be - be1;

        return Tuple.Create(flatWidth + be - bc, be1, be2, criticalWidth);
    }

    /// <summary>
    /// 计算有效宽度，考虑相邻杆件的板组约束系数
    /// </summary>
    /// <param name="flatWidth">平板宽度</param>
    /// <param name="thickness">平板厚度</param>
    /// <param name="flatWidthOfBorder">相邻板件的宽度</param>
    /// <param name="psi">当前板件的应力不均匀系数</param>
    /// <param name="k">当前板件的稳定系数</param>
    /// <param name="kc">相邻板件的稳定系数</param>
    /// <param name="sigma1">受压板件边缘的最大控制力</param> 
    /// <param name="elementType">板件类型，0为非加筋板件，1为部分加筋板件，2为加筋板件</param>
    /// <returns>返回有效宽度和临界宽度(低于这个宽度无需折减)</returns>
    public Tuple<double, double> GetEffectiveWidth(
        double flatWidth,
        double thickness,
        double flatWidthOfBorder,
        double psi,
        double k,
        double kc,
        double sigma1,
        int    elementType) {
        if (thickness == 0) return Tuple.Create(0.0, 0.0);

        var b_tRadio = flatWidth / thickness;                        // 宽厚比// 应力分布不均匀系数
        var alpha    = psi < 0 ? 1.15 : 1.15 - 0.15;                 // 计算系数
        var bc       = psi >= 0 ? flatWidth : flatWidth / (1 - psi); // 板件受压区宽度

        // 板组约束系数计算
        var ksi = flatWidthOfBorder / flatWidth * Math.Sqrt(k / kc);
        // 暂时不考虑板组之间的约束系数
        var k1 = thickness >= 2 ? 1 : ksi <= 1.1 ? 1 / Math.Sqrt(ksi) : 0.11 + 0.93 / Math.Pow(ksi - 0.05, 2);
        k1 = elementType switch {
                 0 => Math.Min(k1, 3.0),
                 1 => Math.Min(k1, 2.4),
                 2 => Math.Min(k1, 1.7),
                 _ => throw new ArgumentOutOfRangeException(nameof(elementType), elementType, null)
             };
        var pho = Math.Sqrt(205.0 * k1 * k / sigma1);

        var critical1 = 18 * alpha * pho;
        var critical2 = 38 * alpha * pho;

        var criticalWidth = // 小于该值时截面全部有效
            critical1 * thickness;
        var effectiveWidth = flatWidth;
        if (b_tRadio > critical1 && b_tRadio <= critical2) {
            effectiveWidth = flatWidth + bc * (Math.Sqrt(21.8 * alpha * pho / b_tRadio) - 0.1) - bc;
        }
        else if (b_tRadio >= critical2) {
            effectiveWidth = flatWidth + bc * 25 * alpha * pho / b_tRadio - bc;
        }

        return Tuple.Create(effectiveWidth, criticalWidth);
    }

#endregion

#region 通用方法区

    // 通知一般属性更新
    private void UpdateGeneralProperties() {
        RaisePropertyChanged(nameof(A));
        RaisePropertyChanged(nameof(B));
        RaisePropertyChanged(nameof(C));
        RaisePropertyChanged(nameof(ABar));
        RaisePropertyChanged(nameof(BBar));
        RaisePropertyChanged(nameof(CBar));
        RaisePropertyChanged(nameof(TheoryBlankLength));
        RaisePropertyChanged(nameof(RealBlankLength));
        RaisePropertyChanged(nameof(Area));
        RaisePropertyChanged(nameof(Ixx));
        RaisePropertyChanged(nameof(Xc));
        RaisePropertyChanged(nameof(Iyy));
        RaisePropertyChanged(nameof(M));

        RaisePropertyChanged(nameof(J));
        RaisePropertyChanged(nameof(Wxx));
        RaisePropertyChanged(nameof(Wyy));
        RaisePropertyChanged(nameof(Zxx));
        RaisePropertyChanged(nameof(Zyy));
        RaisePropertyChanged(nameof(Rxx));
        RaisePropertyChanged(nameof(Ryy));
        RaisePropertyChanged(nameof(X0));
        RaisePropertyChanged(nameof(Cw));
        RaisePropertyChanged(nameof(BetaW));
        RaisePropertyChanged(nameof(BetaF));
        RaisePropertyChanged(nameof(BetaT));

        RaisePropertyChanged(nameof(JJ));


        OnPropertyChanged(); // 事件通知外界属性发生改变
    }

    // 通知有效截面属性更新

#endregion

#region 构造函数

    /// <summary>
    /// 折弯槽钢构造函数
    /// </summary>
    /// <param name="name"></param> C型钢名字
    /// <param name="h"></param> 总高度
    /// <param name="w"></param> 总宽度
    /// <param name="t"></param> 厚度
    /// <param name="r"></param> 中心R角
    public CSection(string name, double h, double w, double t, double r) : this(name, h, w, 0, t, r) {
    }

#endregion

#region 事件委托

    public new event EventHandler PropertyChanged;


    protected virtual void OnPropertyChanged() {
        PropertyChanged?.Invoke(this, EventArgs.Empty);
    }

#endregion
}