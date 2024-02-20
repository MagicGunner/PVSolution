using CADToolBox.Modules.TrackerGA.ViewModels;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace CADToolBox.Modules.TrackerGA.Services.Implement;

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

    public void NavigateTo<T>() where T : ViewModelBase =>
        CurrentViewModel = TrackerApp.Current.Services.GetService<T>();
}