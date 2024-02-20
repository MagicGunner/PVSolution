using System;
using System.Windows;
using CADToolBox.Modules.TrackerGA.Services.Implement;
using CADToolBox.Modules.TrackerGA.ViewModels;
using CADToolBox.Modules.TrackerGA.ViewModels.SubViewModels;
using CADToolBox.Modules.TrackerGA.Views;
using CADToolBox.Modules.TrackerGA.Views.SubViews;
using Microsoft.Extensions.DependencyInjection;

namespace CADToolBox.Modules.TrackerGA;

public class TrackerApp : Application {
    public IServiceProvider Services { get; }

    private static volatile TrackerApp? _current;
    private static readonly object      Lock = new();

    public new static TrackerApp Current {
        get {
            // ��һ�μ�飬���ʵ�������ڣ��������������
            if (_current != null) return _current;
            lock (Lock) {
                // �ڶ��μ�飬ȷ��ֻ��һ���̴߳���ʵ��
                if (_current != null) return _current;
                _current = new TrackerApp();
            }

            return _current;
        }
    }

    public TrackerApp() {
        var container = new ServiceCollection();

        container.AddSingleton<NavigationService>();
        container.AddSingleton<HomeView>();
        container.AddSingleton<HomeViewModel>();

        container.AddTransient<TrackerMainViewModel>();
        container.AddTransient(sp => new TrackerMainView {
                                                             DataContext = sp.GetRequiredService<TrackerMainViewModel>()
                                                         });


        Services = container.BuildServiceProvider();
    }

    public void StartUp() {
        var mainWindow = Services.GetService<TrackerMainView>();
        mainWindow?.Show();
    }
}