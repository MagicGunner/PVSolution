﻿using System;
using System.Collections.Generic;
using System.Linq;
using CADToolBox.Resource.NameDictionarys;
using CADToolBox.Shared.Models.CADModels.Implement;
using CADToolBox.Shared.Models.UIModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.UIModels.Implement;

public partial class PostInfo(PostModel postModel
) : ObservableObject, ITrackerItemInfo {
#region 自身属性

    public PostModel PostModel => postModel;

    [ObservableProperty]
    private bool _isDetailsVisible;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private List<string> _sectionTypeList = GeneralTemplateData.PostSectionMap.Keys.ToList();


    public List<string>? SectionList {
        get {
            return SectionType == null
                       ? null
                       : GeneralTemplateData.PostSectionMap[SectionType].Select(item => item.Name).ToList();
        }
    }

#endregion


#region 立柱模型数据

    public int Num {
        get => postModel.Num;
        set =>
            SetProperty(postModel.Num,
                        value,
                        postModel,
                        (model,
                         value) => model.Num = value);
    }

    public bool IsDrive {
        get => postModel.IsDrive;
        set {
            if (SetProperty(postModel.IsDrive,
                            value,
                            postModel,
                            (model,
                             value) => model.IsDrive = value)) { OnIsDriveChanged(); }
        }
    }

    public bool IsMotor {
        get => postModel.IsMotor;
        set =>
            SetProperty(postModel.IsMotor,
                        value,
                        postModel,
                        (model,
                         value) => model.IsMotor = value);
    }

    public double X {
        get => postModel.X;
        set =>
            SetProperty(postModel.X,
                        value,
                        postModel,
                        (model,
                         value) => model.X = value);
    }

    public string? Section {
        get => postModel.Section;
        set =>
            SetProperty(postModel.Section,
                        value,
                        postModel,
                        (model,
                         value) => model.Section = value);
    }

    public string? SectionType {
        get => postModel.SectionType;
        set =>
            SetProperty(postModel.SectionType,
                        value,
                        postModel,
                        (model,
                         value) => model.SectionType = value);
    }

    public string? Material {
        get => postModel.Material;
        set =>
            SetProperty(postModel.Material,
                        value,
                        postModel,
                        (model,
                         value) => model.Material = value);
    }

    public double PileUpGround {
        get => postModel.PileUpGround;
        set =>
            SetProperty(postModel.PileUpGround,
                        value,
                        postModel,
                        (model,
                         value) => model.PileUpGround = value);
    }

    public double PileDownGround {
        get => postModel.PileDownGround;
        set =>
            SetProperty(postModel.PileDownGround,
                        value,
                        postModel,
                        (model,
                         value) => model.PileDownGround = value);
    }

    public double LeftToBeam {
        get => postModel.LeftToBeam;
        set =>
            SetProperty(postModel.LeftToBeam,
                        value,
                        postModel,
                        (model,
                         value) => model.LeftToBeam = value);
    }

    public double RightToBeam {
        get => postModel.RightToBeam;
        set =>
            SetProperty(postModel.RightToBeam,
                        value,
                        postModel,
                        (model,
                         value) => model.RightToBeam = value);
    }

    public double PileWidth {
        get => postModel.PileWidth;
        set =>
            SetProperty(postModel.PileWidth,
                        value,
                        postModel,
                        (model,
                         value) => model.PileWidth = value);
    }

    public double LeftSpan {
        get => postModel.LeftSpan;
        set {
            if (SetProperty(postModel.LeftSpan,
                            value,
                            postModel,
                            (model,
                             value) => model.LeftSpan = value)) { OnSpanChanged(); }
        }
    }

    public double RightSpan {
        get => postModel.RightSpan;
        set {
            if (SetProperty(postModel.RightSpan,
                            value,
                            postModel,
                            (model,
                             value) => model.RightSpan = value)) { OnSpanChanged(); }
        }
    }

#endregion

#region 事件

    public event EventHandler? SpanChanged;
    public event EventHandler? IsDriveChanged;

    public virtual void OnSpanChanged() {
        SpanChanged?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnIsDriveChanged() {
        IsDriveChanged?.Invoke(this, EventArgs.Empty);
    }

#endregion
}