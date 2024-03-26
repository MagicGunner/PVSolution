using CommunityToolkit.Mvvm.ComponentModel;

namespace SapToolBox.Modules.CommonTools.ViewModels.SubViewModels;

public partial class HomeViewModel : ViewModelBase {
    [ObservableProperty]
    private string _viewName = "通用工具首页";
}