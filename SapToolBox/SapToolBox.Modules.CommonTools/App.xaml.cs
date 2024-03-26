using System;
using SAP2000v1;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SapToolBox.Modules.CommonTools.Services.Implement;
using SapToolBox.Modules.CommonTools.ViewModels;
using SapToolBox.Modules.CommonTools.ViewModels.SubViewModels;
using SapToolBox.Modules.CommonTools.Views;
using SapToolBox.Modules.CommonTools.Views.SubViews;

namespace SapToolBox.Modules.CommonTools;

public partial class App : Application {
    public IServiceProvider Services { get; }

    public new static App Current => (App)Application.Current;

    public cSapModel SapModel { get; set; }

    public App() {
        Services = ConfigureServices();
    }


    protected override void OnStartup(StartupEventArgs e) {
        cHelper myHelper = new Helper();
        var sapObject = myHelper.GetObject("CSI.SAP2000.API.SapObject");
        SapModel = sapObject.SapModel;
        base.OnStartup(e);

        Services.GetRequiredService<MainView>().Show();
    }

    private static IServiceProvider ConfigureServices() {
        var services = new ServiceCollection();
        services.AddSingleton<NavigationService>();


        services.AddTransient<HomeView>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<SectionDefView>();
        services.AddTransient<SectionDefViewModel>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<MainView>(sp => new MainView { DataContext = sp.GetRequiredService<MainViewModel>() });

        return services.BuildServiceProvider();
    }

    public void Run(cSapModel sapModel) {
        SapModel = sapModel;

        Services.GetRequiredService<MainView>().Show();
    }
}