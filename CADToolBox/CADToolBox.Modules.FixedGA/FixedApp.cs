using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using CADToolBox.Modules.FixedGA.Services.Implement;
using CADToolBox.Modules.FixedGA.ViewModels;
using CADToolBox.Modules.FixedGA.ViewModels.SubViewModels;
using CADToolBox.Modules.FixedGA.Views;
using CADToolBox.Modules.FixedGA.Views.SubViews;
using CADToolBox.Shared.Models.CADModels.Implement;
using CADToolBox.Shared.Models.CADModels.Implement.Fixed;
using Microsoft.Extensions.DependencyInjection;


namespace CADToolBox.Modules.FixedGA;

public class FixedApp {
    public IServiceProvider Services { get; }


    public FixedModel? FixedModel { get; set; }

    private static volatile FixedApp? _current;
    private static readonly object    Lock = new();

    public static FixedApp Current {
        get {
            // ��һ�μ�飬���ʵ�������ڣ��������������
            if (_current != null) return _current;
            lock (Lock) {
                // �ڶ��μ�飬ȷ��ֻ��һ���̴߳���ʵ��
                if (_current != null) return _current;
                _current = new FixedApp();
            }

            return _current;
        }
    }


    public FixedApp() {
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

        container.AddTransient<FixedMainViewModel>();
        container.AddTransient<FixedMainView>(sp => new FixedMainView { DataContext = sp.GetRequiredService<FixedMainViewModel>() });

        Services = container.BuildServiceProvider();
    }

    public void Run() {
        Services.GetRequiredService<FixedMainView>().ShowDialog();
    }

    private void Init(FixedModel fixedModel) {
        FixedModel = fixedModel;
    }
}