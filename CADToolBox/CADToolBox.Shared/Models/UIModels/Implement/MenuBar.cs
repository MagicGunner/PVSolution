using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.UIModels.Implement {
    public partial class MenuBar : ObservableObject {
        [ObservableProperty]
        private string? _icon;

        [ObservableProperty]
        private string? _title;
    }
}