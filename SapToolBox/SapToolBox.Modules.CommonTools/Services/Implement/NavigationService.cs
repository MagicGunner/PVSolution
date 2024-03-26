using System;
using Microsoft.Extensions.DependencyInjection;
using SapToolBox.Modules.CommonTools.ViewModels;

namespace SapToolBox.Modules.CommonTools.Services.Implement;

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

    public void NavigateTo<T>() where T : ViewModelBase => CurrentViewModel = App.Current.Services.GetService<T>();
}