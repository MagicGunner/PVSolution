using System;
using CADToolBox.Modules.FixedGA.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CADToolBox.Modules.FixedGA.Services.Implement;

public class NavigationService {
    private ViewModelBase? _currentViewModel;

    public ViewModelBase? CurrentViewModel {
        get => _currentViewModel;
        set {
            _currentViewModel = value;
            // notify property changed
            CurrentViewModelChanged?.Invoke();
        }
    }

    public event Action? CurrentViewModelChanged;

    public void NavigateTo<T>() where T : ViewModelBase => CurrentViewModel = FixedApp.Current.Services.GetService<T>();
}