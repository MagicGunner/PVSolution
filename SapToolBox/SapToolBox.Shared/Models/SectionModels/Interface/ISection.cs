using System;

namespace SapToolBox.Shared.Models.SectionModels.Interface;

public interface ISection
{
    public string? Name     { get; set; } // 截面名称
    public string? Material { get; set; } // 截面材质
    public double  Area     { get; }      // 截面积
    public double  Ixx      { get; }      // 强轴惯性矩
    public double  Iyy      { get; }      // 弱轴惯性矩
    public double  Ixy      { get; }      // 惯性积
    public double  J        { get; }      // 扭转常数
    public double  Wxx      { get; }      // 强轴弹性模量
    public double  Wyy      { get; }      // 弱轴弹性模量
    public double  Zxx      { get; }      // 强轴塑性模量
    public double  Zyy      { get; }      // 弱轴塑性模量
    public double  Rxx      { get; }      // 强轴回转半径
    public double  Ryy      { get; }      // 弱轴回转半径
    public double  X0       { get; }      // 剪心

    public double Cw { get; } // 翘曲惯性矩

    // 定义事件
    event EventHandler PropertyChanged;

    // 计算有效截面属性
    public void SetEffectiveWidth(double sigmaMax, double sigmaMin, double sigma1);
}