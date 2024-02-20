using SapToolBox.Base.DesignCodes.Implement;
using SapToolBox.Base.DesignCodes;
using SapToolBox.Base.Sections.Implement;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System;
using Prism.Mvvm;
using SapToolBox.Base.Sections;

namespace SapToolBox.PreTools.ViewModels.SubViewModels.SectionViewModels;

public class SectionViewModelBase : BindableBase {
    private ObservableCollection<string> _designCodes = ["GB 50018", "AISI"];

    public ObservableCollection<string> DesignCodes {
        get => _designCodes;
        set => SetProperty(ref _designCodes, value);
    }

    private List<object> _designCodeVisible;

    public List<object> DesignCodeVisible {
        get => _designCodeVisible;
        set => SetProperty(ref _designCodeVisible, value);
    }

    private int _selectedCodeIndex = 0;

    public int SelectedCodeIndex {
        get => _selectedCodeIndex;
        set {
            if (!SetProperty(ref _selectedCodeIndex, value)) return;
            UpdateCodeVisible();
            RaisePropertyChanged(nameof(DesignCodeVisible));
        }
    }

    public SectionViewModelBase() {
        // 设计规范的可见性
        DesignCodeVisible = new List<object>(DesignCodes.Count);
        for (var i = 0; i < DesignCodes.Count; i++) { DesignCodeVisible.Add(Visibility.Visible); }

        UpdateCodeVisible();
    }

    private void UpdateCodeVisible() {
        for (var i = 0; i < DesignCodeVisible.Count; i++) {
            if (i == SelectedCodeIndex) { DesignCodeVisible[i] = Visibility.Visible; } else {
                DesignCodeVisible[i] = Visibility.Collapsed;
            }
        }
    }
}