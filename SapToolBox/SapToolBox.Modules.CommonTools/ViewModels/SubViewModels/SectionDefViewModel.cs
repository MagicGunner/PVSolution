using CommunityToolkit.Mvvm.ComponentModel;

namespace SapToolBox.Modules.CommonTools.ViewModels.SubViewModels;

public partial class SectionDefViewModel : ViewModelBase {
    [ObservableProperty]
    private string _viewName = "截面定义辅助页";
}