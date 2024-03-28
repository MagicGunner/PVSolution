using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Prism.Mvvm;
using SapToolBox.Resource.DesignResources;
using SapToolBox.Shared.Models.SectionModels.Implement;
using SapToolBox.Shared.Models.SectionModels.Interface;

namespace SapToolBox.Modules.CommonTools.Models.UIModels;

public class SectionInfo : BindableBase {
#region 字段属性

    public ISection? Section;

    public string? DisplayName { get; set; } // 唯一标识

    private string? _sectionType;

    // 主要处理切换成非预设截面类型的截面规格处理
    public string? SectionType {
        get => _sectionType;
        set {
            if (!SetProperty(ref _sectionType, value)) return;
            if (SectionType == null) return;
            if (GeneralTemplateData.PostSectionMap.TryGetValue(SectionType, out var nameList)) { // 如果截面库中有该种截面类型
                SectionName = nameList?.Select(item => item.Name).First();
            } else {
                switch (SectionType) { // 如果是自定义输入的截面类型，初始化
                    case "焊接H型钢":
                        SectionProps = [
                                           new VariableInfo {
                                                                Name = "H",
                                                                Value = 200
                                                            },
                                           new VariableInfo {
                                                                Name = "B",
                                                                Value = 100
                                                            },
                                           new VariableInfo {
                                                                Name = "tw",
                                                                Value = 3
                                                            },
                                           new VariableInfo {
                                                                Name = "tf",
                                                                Value = 3
                                                            }
                                       ];
                        foreach (var variableInfo in SectionProps) {
                            variableInfo.ValueChanged += (_,
                                                          _) => {
                                                             UpdateSectionName();
                                                         };
                        }

                        UpdateSectionName();
                        break;
                    case "折弯C型钢":
                        SectionProps = [
                                           new VariableInfo {
                                                                Name = "H",
                                                                Value = 210
                                                            },
                                           new VariableInfo {
                                                                Name = "B",
                                                                Value = 100
                                                            },
                                           new VariableInfo {
                                                                Name = "L",
                                                                Value = 30
                                                            },
                                           new VariableInfo {
                                                                Name = "t",
                                                                Value = 3
                                                            }
                                       ];
                        foreach (var variableInfo in SectionProps) {
                            variableInfo.ValueChanged += (_,
                                                          _) => {
                                                             UpdateSectionName();
                                                         };
                        }

                        UpdateSectionName();
                        break;
                    case "折弯槽钢":
                        SectionProps = [
                                           new VariableInfo {
                                                                Name = "H",
                                                                Value = 210
                                                            },
                                           new VariableInfo {
                                                                Name = "B",
                                                                Value = 100
                                                            },
                                           new VariableInfo {
                                                                Name = "t",
                                                                Value = 3
                                                            }
                                       ];
                        foreach (var variableInfo in SectionProps) {
                            variableInfo.ValueChanged += (_,
                                                          _) => {
                                                             UpdateSectionName();
                                                         };
                        }

                        UpdateSectionName();
                        break;
                    case "折弯角钢":
                        SectionProps = [
                                           new VariableInfo {
                                                                Name = "B",
                                                                Value = 63
                                                            },
                                           new VariableInfo {
                                                                Name = "b",
                                                                Value = 45
                                                            },
                                           new VariableInfo {
                                                                Name = "t",
                                                                Value = 3
                                                            }
                                       ];
                        foreach (var variableInfo in SectionProps) {
                            variableInfo.ValueChanged += (_,
                                                          _) => {
                                                             UpdateSectionName();
                                                         };
                        }

                        UpdateSectionName();
                        break;
                    case "折弯几字型钢":
                        SectionProps = [
                                           new VariableInfo {
                                                                Name = "W",
                                                                Value = 79
                                                            },
                                           new VariableInfo {
                                                                Name = "H",
                                                                Value = 60
                                                            },
                                           new VariableInfo {
                                                                Name = "B",
                                                                Value = 30
                                                            },
                                           new VariableInfo {
                                                                Name = "t",
                                                                Value = 3
                                                            }
                                       ];
                        foreach (var variableInfo in SectionProps) {
                            variableInfo.ValueChanged += (_,
                                                          _) => {
                                                             UpdateSectionName();
                                                         };
                        }

                        UpdateSectionName();
                        break;
                    case "方管":
                        SectionProps = [
                                           new VariableInfo {
                                                                Name = "D",
                                                                Value = 140
                                                            },
                                           new VariableInfo {
                                                                Name = "t",
                                                                Value = 3.0
                                                            },
                                           new VariableInfo {
                                                                Name = "r",
                                                                Value = 3.0
                                                            }
                                       ];
                        foreach (var variableInfo in SectionProps) {
                            variableInfo.ValueChanged += (_,
                                                          _) => {
                                                             UpdateSectionName();
                                                         };
                        }

                        UpdateSectionName();
                        break;
                    case "矩形管":
                        SectionProps = [
                                           new VariableInfo {
                                                                Name = "H",
                                                                Value = 200
                                                            },
                                           new VariableInfo {
                                                                Name = "B",
                                                                Value = 100
                                                            },
                                           new VariableInfo {
                                                                Name = "t",
                                                                Value = 3
                                                            },
                                           new VariableInfo {
                                                                Name = "r",
                                                                Value = 3
                                                            }
                                       ];
                        foreach (var variableInfo in SectionProps) {
                            variableInfo.ValueChanged += (_,
                                                          _) => {
                                                             UpdateSectionName();
                                                         };
                        }

                        UpdateSectionName();
                        break;
                }
            }
        }
    }

    private string? _material;

    public string? Material {
        get => _material;
        set => SetProperty(ref _material, value);
    }

    private List<string> _materialList = ["Q235", "Q355", "Q390", "Q420", "Q460", "Q500"];

    public List<string> MaterList {
        get => _materialList;
        set => SetProperty(ref _materialList, value);
    }

    private string? _sectionName;

    // 主要处理热轧型钢规格切换的逻辑
    public string? SectionName {
        get => _sectionName;
        set {
            if (!SetProperty(ref _sectionName, value)) return;
            if (SectionName == null) return;

            switch (SectionType) { // 当截面类型为热轧时SectionProps中的对象不需要添加事件，这种情况手动触发绘图方法
                case "W型钢":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.WSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    UpdateSectionPoints();

                    break;
                case "宽翼缘H型钢(HW)":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollHWSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    UpdateSectionPoints();

                    break;
                case "中翼缘H型钢(HM)":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollHMSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    UpdateSectionPoints();

                    break;
                case "窄翼缘H型钢(HN)":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollHNSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    UpdateSectionPoints();

                    break;
                case "薄壁H型钢(HT)":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollHTSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    UpdateSectionPoints();

                    break;
                case "无缝钢管":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RPileSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    UpdateSectionPoints();

                    break;
                case "焊接钢管":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.WPileSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    UpdateSectionPoints();
                    break;
                case "热轧槽钢":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollCSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    UpdateSectionPoints();

                    break;
                case "热轧等边角钢":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollEqualLSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    UpdateSectionPoints();

                    break;
                case "热轧不等边角钢":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollUnEqualLSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    UpdateSectionPoints();

                    break;
            }
        }
    }

    private bool _isEditable; // 当前截面是否可以编辑

    public bool IsEditable {
        get => _isEditable;
        set => SetProperty(ref _isEditable, value);
    }

    private bool _isClose; // 是否闭口截面，影响前台界面展示

    public bool IsClose {
        get => _isClose;
        set => SetProperty(ref _isClose, value);
    }


    private ObservableCollection<VariableInfo>? _sectionProps;

    public ObservableCollection<VariableInfo>? SectionProps {
        get => _sectionProps;
        set => SetProperty(ref _sectionProps, value);
    }

    private PointCollection? _outerSectionPoints; // 多段线截面绘制关键点

    public PointCollection? OuterSectionPoints {
        get => _outerSectionPoints;
        set => SetProperty(ref _outerSectionPoints, value);
    }

    private PointCollection? _innerSectionPoints; // 多段线截面绘制关键点,当为闭口截面时需要

    public PointCollection? InnerSectionPoints {
        get => _innerSectionPoints;
        set => SetProperty(ref _innerSectionPoints, value);
    }

#endregion


#region 方法区

    // 更新截面点坐标，当截面属性发生改变时(冷弯)，或者截面名称发生改变时(热轧)
    public       double CanvasWidth  = 300;
    public       double CanvasHeight = 300;
    public       int    CireDivNum   = 100; // 将一个整圆划分为100份
    public const double Pi           = Math.PI;


    private void UpdateSectionPoints() {
        if (SectionProps == null) return;
        double H;
        double D;
        double R;
        double B;
        double L;
        double W;
        double b;
        double t;
        double tw;
        double tf;
        switch (SectionType) {
            case "无缝钢管":
            case "焊接钢管":
                IsClose = true;
                D = SectionProps[0].Value;
                t = SectionProps[1].Value;
                OuterSectionPoints = [];
                R = D / 2;
                for (var i = 0; i < CireDivNum; i++) {
                    OuterSectionPoints.Add(new Point(CanvasWidth / 2 + R * Math.Cos(2 * Pi / CireDivNum * i), CanvasWidth / 2 + R * Math.Sin(2 * Pi / CireDivNum * i)));
                }

                R = D / 2 - t;
                InnerSectionPoints = [];
                for (var i = 0; i < CireDivNum; i++) {
                    InnerSectionPoints.Add(new Point(CanvasWidth / 2 + R * Math.Cos(2 * Pi / CireDivNum * i), CanvasWidth / 2 + R * Math.Sin(2 * Pi / CireDivNum * i)));
                }

                break;
            case "方管":
                IsClose = true;
                H = SectionProps[0].Value;
                B = SectionProps[0].Value;
                t = SectionProps[1].Value;
                OuterSectionPoints = new PointCollection {
                                                             new((CanvasWidth - B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight - H) / 2),
                                                             new((CanvasWidth - B) / 2, (CanvasHeight - H) / 2)
                                                         };
                InnerSectionPoints = new PointCollection {
                                                             new((CanvasWidth - B) / 2 + t, (CanvasHeight + H) / 2 - t),
                                                             new((CanvasWidth + B) / 2 - t, (CanvasHeight + H) / 2 - t),
                                                             new((CanvasWidth + B) / 2 - t, (CanvasHeight - H) / 2 + t),
                                                             new((CanvasWidth - B) / 2 + t, (CanvasHeight - H) / 2 + t)
                                                         };
                break;
            case "矩形管":
                IsClose = true;
                H = SectionProps[0].Value;
                B = SectionProps[1].Value;
                t = SectionProps[2].Value;
                OuterSectionPoints = new PointCollection {
                                                             new((CanvasWidth - B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight - H) / 2),
                                                             new((CanvasWidth - B) / 2, (CanvasHeight - H) / 2)
                                                         };
                InnerSectionPoints = new PointCollection {
                                                             new((CanvasWidth - B) / 2 + t, (CanvasHeight + H) / 2 - t),
                                                             new((CanvasWidth + B) / 2 - t, (CanvasHeight + H) / 2 - t),
                                                             new((CanvasWidth + B) / 2 - t, (CanvasHeight - H) / 2 + t),
                                                             new((CanvasWidth - B) / 2 + t, (CanvasHeight - H) / 2 + t)
                                                         };
                break;
            case "折弯C型钢":
                IsClose = false;
                H = SectionProps[0].Value;
                B = SectionProps[1].Value;
                L = SectionProps[2].Value;
                t = SectionProps[3].Value;
                OuterSectionPoints = new PointCollection {
                                                             new((CanvasWidth - B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight + H) / 2 - L),
                                                             new((CanvasWidth + B) / 2 - t, (CanvasHeight + H) / 2 - L),
                                                             new((CanvasWidth + B) / 2 - t, (CanvasHeight + H) / 2 - t),
                                                             new((CanvasWidth - B) / 2 + t, (CanvasHeight + H) / 2 - t),
                                                             new((CanvasWidth - B) / 2 + t, (CanvasHeight - H) / 2 + t),
                                                             new((CanvasWidth + B) / 2 - t, (CanvasHeight - H) / 2 + t),
                                                             new((CanvasWidth + B) / 2 - t, (CanvasHeight - H) / 2 + L),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight - H) / 2 + L),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight - H) / 2),
                                                             new((CanvasWidth - B) / 2, (CanvasHeight - H) / 2)
                                                         };
                break;
            case "折弯槽钢":
                IsClose = false;
                H = SectionProps[0].Value;
                B = SectionProps[1].Value;
                t = SectionProps[2].Value;
                OuterSectionPoints = new PointCollection {
                                                             new((CanvasWidth - B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight + H) / 2 - t),
                                                             new((CanvasWidth - B) / 2 + t, (CanvasHeight + H) / 2 - t),
                                                             new((CanvasWidth - B) / 2 + t, (CanvasHeight - H) / 2 + t),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight - H) / 2 + t),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight - H) / 2),
                                                             new((CanvasWidth - B) / 2, (CanvasHeight - H) / 2),
                                                         };
                break;
            case "折弯几字型钢":
                IsClose = false;
                W = SectionProps[0].Value;
                H = SectionProps[1].Value;
                B = SectionProps[2].Value;
                t = SectionProps[3].Value;
                OuterSectionPoints = new PointCollection {
                                                             new((CanvasWidth - B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight - H) / 2 + t),
                                                             new((CanvasWidth + W) / 2, (CanvasHeight - H) / 2 + t),
                                                             new((CanvasWidth + W) / 2, (CanvasHeight - H) / 2),
                                                             new((CanvasWidth + B) / 2 - t, (CanvasHeight - H) / 2),
                                                             new((CanvasWidth + B) / 2 - t, (CanvasHeight + H) / 2 - t),
                                                             new((CanvasWidth - B) / 2 + t, (CanvasHeight + H) / 2 - t),
                                                             new((CanvasWidth - B) / 2 + t, (CanvasHeight - H) / 2),
                                                             new((CanvasWidth - W) / 2, (CanvasHeight - H) / 2),
                                                             new((CanvasWidth - W) / 2, (CanvasHeight - H) / 2 + t),
                                                             new((CanvasWidth - B) / 2, (CanvasHeight - H) / 2 + t),
                                                         };
                break;
            case "热轧等边角钢":
                IsClose = false;
                b = SectionProps[0].Value;
                t = SectionProps[1].Value;
                OuterSectionPoints = new PointCollection {
                                                             new((CanvasWidth - b) / 2, (CanvasHeight + b) / 2),
                                                             new((CanvasWidth + b) / 2, (CanvasHeight + b) / 2),
                                                             new((CanvasWidth + b) / 2, (CanvasHeight + b) / 2 - t),
                                                             new((CanvasWidth - b) / 2 + t, (CanvasHeight + b) / 2 - t),
                                                             new((CanvasWidth - b) / 2 + t, (CanvasHeight - b) / 2),
                                                             new((CanvasWidth - b) / 2, (CanvasHeight - b) / 2),
                                                         };
                break;
            case "热轧不等边角钢":
                IsClose = false;
                B = SectionProps[0].Value;
                b = SectionProps[1].Value;
                t = SectionProps[2].Value;
                OuterSectionPoints = new PointCollection {
                                                             new((CanvasWidth - b) / 2, (CanvasHeight + B) / 2),
                                                             new((CanvasWidth + b) / 2, (CanvasHeight + B) / 2),
                                                             new((CanvasWidth + b) / 2, (CanvasHeight + B) / 2 - t),
                                                             new((CanvasWidth - b) / 2 + t, (CanvasHeight + B) / 2 - t),
                                                             new((CanvasWidth - b) / 2 + t, (CanvasHeight - B) / 2),
                                                             new((CanvasWidth - b) / 2, (CanvasHeight - B) / 2),
                                                         };
                break;
            case "折弯角钢":
                IsClose = false;
                B = SectionProps[0].Value;
                b = SectionProps[1].Value;
                t = SectionProps[2].Value;
                OuterSectionPoints = new PointCollection {
                                                             new((CanvasWidth - b) / 2, (CanvasHeight + B) / 2),
                                                             new((CanvasWidth + b) / 2, (CanvasHeight + B) / 2),
                                                             new((CanvasWidth + b) / 2, (CanvasHeight + B) / 2 - t),
                                                             new((CanvasWidth - b) / 2 + t, (CanvasHeight + B) / 2 - t),
                                                             new((CanvasWidth - b) / 2 + t, (CanvasHeight - B) / 2),
                                                             new((CanvasWidth - b) / 2, (CanvasHeight - B) / 2),
                                                         };
                break;
            case "W型钢":
                IsClose = false;
                H = SectionProps[0].Value;
                B = SectionProps[1].Value;
                tw = SectionProps[2].Value;
                tf = SectionProps[3].Value;
                OuterSectionPoints = new PointCollection {
                                                             new((CanvasWidth - B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight + H) / 2 - tf),
                                                             new((CanvasWidth + tw) / 2, (CanvasHeight + H) / 2 - tf),
                                                             new((CanvasWidth + tw) / 2, (CanvasHeight - H) / 2 + tf),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight - H) / 2 + tf),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight - H) / 2),
                                                             new((CanvasWidth - B) / 2, (CanvasHeight - H) / 2),
                                                             new((CanvasWidth - B) / 2, (CanvasHeight - H) / 2 + tf),
                                                             new((CanvasWidth - tw) / 2, (CanvasHeight - H) / 2 + tf),
                                                             new((CanvasWidth - tw) / 2, (CanvasHeight + H) / 2 - tf),
                                                             new((CanvasWidth - B) / 2, (CanvasHeight + H) / 2 - tf),
                                                         };
                break;
            case "焊接H型钢":
                IsClose = false;
                H = SectionProps[0].Value;
                B = SectionProps[1].Value;
                tw = SectionProps[2].Value;
                tf = SectionProps[3].Value;
                OuterSectionPoints = new PointCollection {
                                                             new((CanvasWidth - B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight + H) / 2),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight + H) / 2 - tf),
                                                             new((CanvasWidth + tw) / 2, (CanvasHeight + H) / 2 - tf),
                                                             new((CanvasWidth + tw) / 2, (CanvasHeight - H) / 2 + tf),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight - H) / 2 + tf),
                                                             new((CanvasWidth + B) / 2, (CanvasHeight - H) / 2),
                                                             new((CanvasWidth - B) / 2, (CanvasHeight - H) / 2),
                                                             new((CanvasWidth - B) / 2, (CanvasHeight - H) / 2 + tf),
                                                             new((CanvasWidth - tw) / 2, (CanvasHeight - H) / 2 + tf),
                                                             new((CanvasWidth - tw) / 2, (CanvasHeight + H) / 2 - tf),
                                                             new((CanvasWidth - B) / 2, (CanvasHeight + H) / 2 - tf),
                                                         };
                break;
        }
    }

    // 对于非预设截面的类型，当前台截面数据发生改变时更新截面名称
    private void UpdateSectionName() {
        if (SectionProps == null) return;
        switch (SectionType) {
            case "焊接H型钢":
                SectionName = "H";
                for (var i = 0; i < SectionProps.Count; i++) {
                    var num = SectionProps[i].Value;
                    if (SectionProps[i].Name == "tw" || SectionProps[i].Name == "tf") {
                        SectionName += num * 100 % 10 == 0 ? $"{num:F1}" : num; // 厚度不足一位小数补成一位小数
                    } else {
                        SectionName += num;
                    }

                    if (i != SectionProps.Count - 1) {
                        SectionName += "x";
                    }
                }

                break;
            case "折弯C型钢":
                SectionName = "C";
                for (var i = 0; i < SectionProps.Count; i++) {
                    var num = SectionProps[i].Value;
                    if (SectionProps[i].Name == "t") {
                        SectionName += num * 100 % 10 == 0 ? $"{num:F1}" : num; // 厚度不足一位小数补成一位小数
                    } else {
                        SectionName += num;
                    }

                    if (i != SectionProps.Count - 1) {
                        SectionName += "x";
                    }
                }

                break;
            case "折弯槽钢":
                SectionName = "C";
                for (var i = 0; i < SectionProps.Count; i++) {
                    var num = SectionProps[i].Value;
                    if (SectionProps[i].Name == "t") {
                        SectionName += num * 100 % 10 == 0 ? $"{num:F1}" : num; // 厚度不足一位小数补成一位小数
                    } else {
                        SectionName += num;
                    }

                    if (i != SectionProps.Count - 1) {
                        SectionName += "x";
                    }
                }

                break;
            case "折弯角钢":
                SectionName = "L";
                for (var i = 0; i < SectionProps.Count; i++) {
                    var num = SectionProps[i].Value;
                    if (SectionProps[i].Name == "t") {
                        SectionName += num * 100 % 10 == 0 ? $"{num:F1}" : num; // 厚度不足一位小数补成一位小数
                    } else {
                        SectionName += num;
                    }

                    if (i != SectionProps.Count - 1) {
                        SectionName += "x";
                    }
                }

                break;
            case "折弯几字型钢":
                SectionName = "几";
                for (var i = 0; i < SectionProps.Count; i++) {
                    var num = SectionProps[i].Value;
                    if (SectionProps[i].Name == "t") {
                        SectionName += num * 100 % 10 == 0 ? $"{num:F1}" : num; // 厚度不足一位小数补成一位小数
                    } else {
                        SectionName += num;
                    }

                    if (i != SectionProps.Count - 1) {
                        SectionName += "x";
                    }
                }

                break;
            case "方管":
                SectionName = "ST";
                for (var i = 0; i < SectionProps.Count - 1; i++) {
                    var num = SectionProps[i].Value;
                    if (SectionProps[i].Name == "t") {
                        SectionName += num * 100 % 10 == 0 ? $"{num:F1}" : num; // 厚度不足一位小数补成一位小数
                    } else {
                        SectionName += num;
                    }

                    if (i != SectionProps.Count - 2) {
                        SectionName += "x";
                    }
                }

                break;
            case "矩形管":
                SectionName = "ST";
                for (var i = 0; i < SectionProps.Count - 1; i++) {
                    var num = SectionProps[i].Value;
                    if (SectionProps[i].Name == "t") {
                        SectionName += num * 100 % 10 == 0 ? $"{num:F1}" : num; // 厚度不足一位小数补成一位小数
                    } else {
                        SectionName += num;
                    }

                    if (i != SectionProps.Count - 2) {
                        SectionName += "x";
                    }
                }

                break;
        }

        UpdateSectionPoints();
    }


    // 向Sap中添加截面前初始化ISection
    public void InitISection() {
        if (DisplayName == null || SectionProps == null) return;

        switch (SectionType) {
            case "W型钢":
                Section = new HSection(DisplayName, SectionProps[0].Value, SectionProps[1].Value, SectionProps[2].Value, SectionProps[3].Value);
                Section.Material = Material;
                break;
            case "折弯C型钢":
                Section = new CSection(DisplayName, SectionProps[0].Value, SectionProps[1].Value, SectionProps[2].Value, SectionProps[3].Value, SectionProps[3].Value);
                Section.Material = Material;
                break;
        }
    }

#endregion

#region 内部类

    public class VariableInfo : BindableBase {
        public string? Name { get; set; }

        private double _value;

        public double Value {
            get => _value;
            set {
                if (SetProperty(ref _value, value)) {
                    OnValueChanged();
                }
            }
        }

        public event EventHandler? ValueChanged;

        protected virtual void OnValueChanged() {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

#endregion
}