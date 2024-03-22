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