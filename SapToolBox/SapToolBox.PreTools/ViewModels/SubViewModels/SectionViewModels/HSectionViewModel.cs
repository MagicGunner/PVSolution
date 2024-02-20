using Prism.Mvvm;
using SapToolBox.Base.Sections.Implement;

namespace SapToolBox.PreTools.ViewModels.SubViewModels.SectionViewModels;

public class HSectionViewModel : BindableBase {
    private HSection _currentHSection;

    public HSection CurrentHSection {
        get => _currentHSection;
        set => SetProperty(ref _currentHSection, value);
    }

    public HSectionViewModel() {
        CurrentHSection = new HSection("H200x100x3.0x4.0", 200, 100, 4.0, 3.0, 0);
    }
}