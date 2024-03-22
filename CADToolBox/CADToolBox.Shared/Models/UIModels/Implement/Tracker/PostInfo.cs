using System;
using System.Collections.Generic;
using System.Linq;
using CADToolBox.Resource.NameDictionarys;
using CADToolBox.Shared.Models.CADModels.Implement.Tracker;
using CADToolBox.Shared.Models.UIModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.UIModels.Implement.Tracker;

public partial class PostInfo(
    PostModel postModel
) : ObservableObject, ITrackerItemInfo {
    #region 自身属性

    [ObservableProperty]
    private PostModel _postModel = postModel;

    [ObservableProperty]
    private bool _isDetailsVisible;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private List<string> _sectionTypeList = GeneralTemplateData.PostSectionMap.Keys.ToList();


    public List<string>? SectionList {
        get { return SectionType == null ? null : GeneralTemplateData.PostSectionMap[SectionType].Select(item => item.Name).ToList(); }
    }

    #endregion

    #region 构造函数

    //PostModel = postModel;

    #endregion

    #region 立柱模型数据

    public int Num {
        get => PostModel.Num;
        set => SetProperty<PostModel, int>(PostModel.Num, value, PostModel, (model, value) => model.Num = value);
    }

    public bool IsDrive {
        get => PostModel.IsDrive;
        set {
            if (SetProperty<PostModel, bool>(PostModel.IsDrive, value, PostModel, (model, value) => model.IsDrive = value)) {
                OnIsDriveChanged();
            }
        }
    }

    public bool IsMotor {
        get => PostModel.IsMotor;
        set => SetProperty<PostModel, bool>(PostModel.IsMotor, value, PostModel, (model, value) => model.IsMotor = value);
    }

    public double X {
        get => PostModel.X;
        set => SetProperty<PostModel, double>(PostModel.X, value, PostModel, (model, value) => model.X = value);
    }

    public string? Section {
        get => PostModel.Section;
        set => SetProperty<PostModel, string?>(PostModel.Section, value, PostModel, (model, value) => model.Section = value);
    }

    public string? SectionType {
        get => PostModel.SectionType;
        set => SetProperty<PostModel, string?>(PostModel.SectionType, value, PostModel, (model, value) => model.SectionType = value);
    }

    public string? Material {
        get => PostModel.Material;
        set => SetProperty<PostModel, string?>(PostModel.Material, value, PostModel, (model, value) => model.Material = value);
    }

    public double PileUpGround {
        get => PostModel.PileUpGround;
        set => SetProperty<PostModel, double>(PostModel.PileUpGround, value, PostModel, (model, value) => model.PileUpGround = value);
    }

    public double FlangeThickness {
        get => PostModel.FlangeThickness;
        set { SetProperty<PostModel, double>(PostModel.FlangeThickness, value, PostModel, (model, value) => model.FlangeThickness = value); }
    }

    public double PileDownGround {
        get => PostModel.PileDownGround;
        set => SetProperty<PostModel, double>(PostModel.PileDownGround, value, PostModel, (model, value) => model.PileDownGround = value);
    }

    public double LeftToBeam {
        get => PostModel.LeftToBeam;
        set => SetProperty<PostModel, double>(PostModel.LeftToBeam, value, PostModel, (model, value) => model.LeftToBeam = value);
    }

    public double RightToBeam {
        get => PostModel.RightToBeam;
        set => SetProperty<PostModel, double>(PostModel.RightToBeam, value, PostModel, (model, value) => model.RightToBeam = value);
    }

    public double PileWidth {
        get => PostModel.PileWidth;
        set => SetProperty<PostModel, double>(PostModel.PileWidth, value, PostModel, (model, value) => model.PileWidth = value);
    }

    public double LeftSpan {
        get => PostModel.LeftSpan;
        set {
            if (SetProperty<PostModel, double>(PostModel.LeftSpan, value, PostModel, (model, value) => model.LeftSpan = value)) {
                OnLeftSpanChanged();
            }
        }
    }

    public double RightSpan {
        get => PostModel.RightSpan;
        set {
            if (SetProperty<PostModel, double>(PostModel.RightSpan, value, PostModel, (model, value) => model.RightSpan = value)) {
                OnRightSpanChanged();
            }
        }
    }

    #endregion

    #region 事件

    public event EventHandler? LeftSpanChanged;
    public event EventHandler? RightSpanChanged;
    public event EventHandler? IsDriveChanged;

    protected virtual void OnLeftSpanChanged() {
        LeftSpanChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnRightSpanChanged() {
        RightSpanChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnIsDriveChanged() {
        IsDriveChanged?.Invoke(this, EventArgs.Empty);
    }


    public void OnPostModelChanged() {
        OnPropertyChanged(nameof(Num));
        OnPropertyChanged(nameof(IsDrive));
        OnPropertyChanged(nameof(IsMotor));
        OnPropertyChanged(nameof(X));
        OnPropertyChanged(nameof(SectionType));
        OnPropertyChanged(nameof(Section));
        OnPropertyChanged(nameof(Material));
        OnPropertyChanged(nameof(PileUpGround));
        OnPropertyChanged(nameof(PileDownGround));
        OnPropertyChanged(nameof(LeftToBeam));
        OnPropertyChanged(nameof(RightToBeam));
        OnPropertyChanged(nameof(LeftSpan));
        OnPropertyChanged(nameof(RightSpan));
        OnPropertyChanged(nameof(PileWidth));
    }

    #endregion
}