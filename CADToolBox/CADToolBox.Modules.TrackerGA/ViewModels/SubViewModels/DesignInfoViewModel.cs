using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows.Media;
using System;
using System.Windows;
using CADToolBox.Shared.Models.CADModels.Implement;
using Microsoft.Extensions.DependencyInjection;

namespace CADToolBox.Modules.TrackerGA.ViewModels.SubViewModels;

public partial class DesignInfoViewModel : ViewModelBase {
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Test))]
    private TrackerModel? _trackerModel;

    public DesignInfoViewModel() {
        TrackerModel = TrackerApp.Current.TrackerModel;
    }

    private void Test() {
        MessageBox.Show("1111");
    }
}