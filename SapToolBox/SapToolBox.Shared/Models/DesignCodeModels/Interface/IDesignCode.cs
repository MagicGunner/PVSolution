using SapToolBox.Shared.Models.SectionModels.Interface;

namespace SapToolBox.Shared.Models.DesignCodeModels.Interface;

public interface IDesignCode {
    public ISection Section { get; }

    // 材料属性
    public double E  { get; }
    public double Fu { get; }
    public double Fy { get; }
    public double Fb { get; }
    public double Fv { get; }

    // 长度系数等
    public double UnbracedLength { get; }
    public double LengthFactorX  { get; }
    public double LengthFactorY  { get; }

    // 杆件内力
    public double P_1  { get; }
    public double P_2  { get; }
    public double Mx_1 { get; }
    public double Mx_2 { get; }
    public double My_1 { get; }
    public double My_2 { get; }
    public double Vx_1 { get; }
    public double Vx_2 { get; }
    public double Vy_1 { get; }
    public double Vy_2 { get; }
    public double T_1  { get; }
    public double T_2  { get; }

    public void UpdateGeneralProperties();
    public void UpdateDesignProperties();
    public void SetSectionEffectiveWidth();
}