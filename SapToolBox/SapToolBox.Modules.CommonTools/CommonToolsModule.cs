using System;
using Prism.Ioc;
using Prism.Modularity;
using SAP2000v1;
using SapToolBox.Modules.CommonTools.ViewModels;
using SapToolBox.Modules.CommonTools.ViewModels.SubViewModels;
using SapToolBox.Modules.CommonTools.Views;
using SapToolBox.Modules.CommonTools.Views.SubViews;

namespace SapToolBox.Modules.CommonTools;

public class CommonToolsModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        containerRegistry.RegisterSingleton<CommonToolsIndexViewModel>();

        containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
        containerRegistry.RegisterForNavigation<SectionDefView, SectionDefViewModel>();
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}