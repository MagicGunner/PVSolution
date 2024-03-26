using System;
using Prism.Ioc;
using Prism.Modularity;
using SAP2000v1;
using SapToolBox.Modules.CommonTools.Services.Implement;
using SapToolBox.Modules.CommonTools.ViewModels.SubViewModels;
using SapToolBox.Modules.CommonTools.ViewModels;
using SapToolBox.Modules.CommonTools.Views;
using SapToolBox.Modules.CommonTools.Views.SubViews;

namespace SapToolBox.Modules.CommonTools;

public class CommonTools : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        containerRegistry.RegisterSingleton<MainView>();
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}