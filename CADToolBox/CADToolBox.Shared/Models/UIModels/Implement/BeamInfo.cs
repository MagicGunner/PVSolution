﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using CADToolBox.Resource.NameDictionarys;
using CADToolBox.Shared.Models.CADModels.Implement;
using CADToolBox.Shared.Models.CADModels.Interface;
using CADToolBox.Shared.Models.UIModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using static System.Windows.Forms.AxHost;

namespace CADToolBox.Shared.Models.UIModels.Implement;

public partial class BeamInfo : ObservableObject, ITrackerItemInfo {
    #region 自身属性

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Num))]
    [NotifyPropertyChangedFor(nameof(StartX))]
    [NotifyPropertyChangedFor(nameof(EndX))]
    [NotifyPropertyChangedFor(nameof(SectionType))]
    [NotifyPropertyChangedFor(nameof(Section))]
    [NotifyPropertyChangedFor(nameof(Material))]
    [NotifyPropertyChangedFor(nameof(LeftToPre))]
    [NotifyPropertyChangedFor(nameof(RightToNext))]
    [NotifyPropertyChangedFor(nameof(Length))]
    private BeamModel _beamModel = null!;


    [ObservableProperty]
    private bool _isDetailsVisible; // 细节是否展示

    [ObservableProperty]
    private bool _isSelected; // 当前主梁是否被选中

    [ObservableProperty]
    private List<string> _sectionTypeList = GeneralTemplateData.BeamSectionMap.Keys.ToList();

    public List<string>? SectionList {
        get {
            return SectionType == null
                       ? null
                       : GeneralTemplateData.BeamSectionMap[SectionType].Select(item => item.Name).ToList();
        }
    }

    #endregion

    #region 主梁模型属性

    public int Num {
        get => BeamModel.Num;
        set =>
            SetProperty(BeamModel.Num, value, BeamModel, (model,
                                                          value) => model.Num = value);
    }

    public double StartX {
        get => BeamModel.StartX;
        set =>
            SetProperty(BeamModel.StartX, value, BeamModel, (model,
                                                             value) => model.StartX = value);
    }

    public double EndX {
        get => BeamModel.EndX;
        set =>
            SetProperty(BeamModel.EndX, value, BeamModel, (model,
                                                           value) => model.EndX = value);
    }

    public string? SectionType {
        get => BeamModel.SectionType;
        set =>
            SetProperty(BeamModel.SectionType, value, BeamModel, (model,
                                                                  value) => model.SectionType = value);
    }

    public string? Section {
        get => BeamModel.Section;
        set =>
            SetProperty(BeamModel.Section, value, BeamModel, (model,
                                                              value) => model.Section = value);
    }

    public string? Material {
        get => BeamModel.Material;
        set =>
            SetProperty(BeamModel.Material, value, BeamModel, (model,
                                                               value) => model.Material = value);
    }

    public double LeftToPre {
        get => BeamModel.LeftToPre;
        set =>
            SetProperty(BeamModel.LeftToPre, value, BeamModel, (model,
                                                                value) => model.LeftToPre = value);
    }

    public double RightToNext {
        get => BeamModel.RightToNext;
        set =>
            SetProperty(BeamModel.RightToNext, value, BeamModel, (model,
                                                                  value) => model.RightToNext = value);
    }

    public double Length {
        get => BeamModel.Length;
        set {
            if (SetProperty(BeamModel.Length, value, BeamModel, (model,
                                                                 value) => model.Length = value)) {
                OnLengthChanged();
            }
        }
    }

    #endregion

    #region 构造函数

    public BeamInfo(BeamModel beamModel) {
        BeamModel                 =  beamModel;
        BeamModel.PropertyChanged += OnBeamModelChanged;
    }

    #endregion

    #region 事件

    public event EventHandler? LengthChanged;

    protected void OnLengthChanged() {
        LengthChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnBeamModelChanged(object                   sender,
                                    PropertyChangedEventArgs e) {
        OnPropertyChanged(nameof(Num));
        OnPropertyChanged(nameof(StartX));
        OnPropertyChanged(nameof(EndX));
        OnPropertyChanged(nameof(SectionType));
        OnPropertyChanged(nameof(Section));
        OnPropertyChanged(nameof(Material));
        OnPropertyChanged(nameof(LeftToPre));
        OnPropertyChanged(nameof(RightToNext));
        OnPropertyChanged(nameof(Length));
    }

    #endregion
}