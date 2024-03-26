using Prism.Mvvm;

namespace SapToolBox.Shared.Models.UIModels.Implement;

public partial class MenuBar : BindableBase {
    private string? _icon;

    public string? Icon {
        get => _icon;
        set => SetProperty(ref _icon, value);
    }

    private string? _title;

    public string? Title {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string? _nameSpace;

    public string? NameSpace {
        get => _nameSpace;
        set => SetProperty(ref _nameSpace, value);
    }
}