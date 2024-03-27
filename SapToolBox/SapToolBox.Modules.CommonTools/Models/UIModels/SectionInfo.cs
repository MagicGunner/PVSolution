using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;
using SapToolBox.Resource.DesignResources;
using SapToolBox.Shared.Models.SectionModels.Interface;

namespace SapToolBox.Modules.CommonTools.Models.UIModels;

public class SectionInfo : BindableBase {
    #region 字段属性

    public ISection? Section;

    private string? _sectionType;

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
                }
            }
        }
    }

    private string? _material;

    public string? Material {
        get => _material;
        set => SetProperty(ref _material, value);
    }

    private string? _sectionName;

    public string? SectionName {
        get => _sectionName;
        set {
            if (!SetProperty(ref _sectionName, value)) return;
            if (SectionName == null) return;
            switch (SectionType) {
                case "W型钢":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.WSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    break;
                case "宽翼缘H型钢(HW)":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollHWSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    break;
                case "中翼缘H型钢(HM)":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollHMSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    break;
                case "窄翼缘H型钢(HN)":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollHNSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    break;
                case "薄壁H型钢(HT)":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollHTSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    break;
                case "无缝钢管":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RPileSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    break;
                case "焊接钢管":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.WPileSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    break;
                case "热轧槽钢":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollCSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    break;
                case "热轧等边角钢":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollEqualLSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    break;
                case "热轧不等边角钢":
                    SectionProps = [];
                    foreach (var pair in GeneralTemplateData.RollUnEqualLSectionPropDic[SectionName]) {
                        SectionProps.Add(new VariableInfo {
                                                              Name = pair.Key,
                                                              Value = Convert.ToDouble(pair.Value)
                                                          });
                    }

                    break;
            }
        }
    }

    private bool _isEditable; // 当前截面是否可以编辑

    public bool IsEditable {
        get => _isEditable;
        set => SetProperty(ref _isEditable, value);
    }

    private ObservableCollection<VariableInfo>? _sectionProps;

    public ObservableCollection<VariableInfo>? SectionProps {
        get => _sectionProps;
        set => SetProperty(ref _sectionProps, value);
    }

    #endregion


    #region 方法区

    private void UpdateSectionName() {
        if (SectionProps == null) return;
        switch (SectionType) {
            case "焊接H型钢":
                SectionName = "H";
                for (var i = 0; i < SectionProps.Count; i++) {
                    SectionName += SectionProps[i].Value;
                    if (i != SectionProps.Count - 1) {
                        SectionName += "x";
                    }
                }

                break;
            case "折弯C型钢":
                SectionName = "C";
                for (var i = 0; i < SectionProps.Count; i++) {
                    SectionName += SectionProps[i].Value;
                    if (i != SectionProps.Count - 1) {
                        SectionName += "x";
                    }
                }

                break;
            case "折弯角钢":
                SectionName = "L";
                for (var i = 0; i < SectionProps.Count; i++) {
                    SectionName += SectionProps[i].Value;
                    if (i != SectionProps.Count - 1) {
                        SectionName += "x";
                    }
                }

                break;
            case "折弯几字型钢":
                SectionName = "几";
                for (var i = 0; i < SectionProps.Count; i++) {
                    SectionName += SectionProps[i].Value;
                    if (i != SectionProps.Count - 1) {
                        SectionName += "x";
                    }
                }

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

