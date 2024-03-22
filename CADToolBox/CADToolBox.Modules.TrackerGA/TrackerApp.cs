using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using CADToolBox.Modules.TrackerGA.Services.Implement;
using CADToolBox.Modules.TrackerGA.ViewModels;
using CADToolBox.Modules.TrackerGA.ViewModels.SubViewModels;
using CADToolBox.Modules.TrackerGA.Views;
using CADToolBox.Modules.TrackerGA.Views.SubViews;
using CADToolBox.Shared.Models.CADModels.Implement.Tracker;
using Microsoft.Extensions.DependencyInjection;

namespace CADToolBox.Modules.TrackerGA;

public class TrackerApp {
    public IServiceProvider Services { get; }

    public TrackerModel? TrackerModel { get; set; }

    private static volatile TrackerApp? _current;
    private static readonly object      Lock = new();

    public static TrackerApp Current {
        get {
            // 第一次检查，如果实例不存在，则进入锁定区域
            if (_current != null) return _current;
            lock (Lock) {
                // 第二次检查，确保只有一个线程创建实例
                if (_current != null) return _current;
                _current = new TrackerApp();
            }

            return _current;
        }
    }

    public TrackerApp() {
        var container = new ServiceCollection();

        container.AddSingleton<NavigationService>();

        container.AddTransient<HomeView>();
        container.AddTransient<HomeViewModel>();
        container.AddTransient<DesignInfoView>();
        container.AddTransient<DesignInfoViewModel>();
        container.AddTransient<SpanInfoView>();
        container.AddTransient<SpanInfoViewModel>();
        container.AddTransient<SummaryView>();
        container.AddTransient<SummaryViewModel>();


        container.AddTransient<TrackerMainViewModel>();
        container.AddTransient<TrackerMainView>(sp => new TrackerMainView {
                                                                              DataContext = sp
                                                                                 .GetRequiredService<
                                                                                      TrackerMainViewModel>()
                                                                          });
        Services = container.BuildServiceProvider();
    }

    public void Run() {
        Services.GetRequiredService<TrackerMainView>().ShowDialog();
    }

    private void Init(TrackerModel trackerModel) {
        TrackerModel = trackerModel;
    }
}