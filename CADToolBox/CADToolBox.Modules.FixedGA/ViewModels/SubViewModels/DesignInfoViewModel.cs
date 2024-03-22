using CADToolBox.Shared.Models.CADModels.Implement.Fixed;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Modules.FixedGA.ViewModels.SubViewModels;

public partial class DesignInfoViewModel : ViewModelBase {
    [ObservableProperty]
    private FixedModel? _fixedModel;

    public DesignInfoViewModel() {
        _fixedModel = FixedApp.Current.FixedModel;
        Draw();
    }

    private void Draw() {
    }
}