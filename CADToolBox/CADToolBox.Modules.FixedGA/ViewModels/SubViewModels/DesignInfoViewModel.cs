using System.ComponentModel;
using System.Windows;
using CADToolBox.Shared.Models.CADModels.Implement.Fixed;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Modules.FixedGA.ViewModels.SubViewModels;

public partial class DesignInfoViewModel : ViewModelBase {
    [ObservableProperty]
    private FixedModel? _fixedModel;

    public DesignInfoViewModel() {
        FixedModel = FixedApp.Current.FixedModel!;
        FixedModel.PropertyChanged += OnFixModelChanged;
        Draw();
    }

    private void Draw() {
    }

    private void OnFixModelChanged(object sender, PropertyChangedEventArgs e) {
        MessageBox.Show("固定支架模型发生改变");
    }
}