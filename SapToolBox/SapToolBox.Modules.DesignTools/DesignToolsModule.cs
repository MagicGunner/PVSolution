using Prism.Ioc;
using Prism.Modularity;
using SapToolBox.Modules.DesignTools.ViewModels;
using SapToolBox.Modules.DesignTools.ViewModels.SubViewModels;
using SapToolBox.Modules.DesignTools.ViewModels.SubViewModels.SectionViewModels;
using SapToolBox.Modules.DesignTools.Views;
using SapToolBox.Modules.DesignTools.Views.SubViews;
using SapToolBox.Modules.DesignTools.Views.SubViews.SectionViews;

namespace SapToolBox.Modules.DesignTools {
    public class DesignToolsModule : IModule {
        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterSingleton<DesignToolsIndexViewModel>();

            containerRegistry.RegisterForNavigation<DesignToolsIndexView, DesignToolsIndexViewModel>();
            containerRegistry.RegisterForNavigation<DesignOverwriteView, DesignOverwriteViewModel>();
            containerRegistry.RegisterForNavigation<SectionDesignerView, SectionDesignerViewModel>();
            containerRegistry.RegisterForNavigation<HSectionView, HSectionViewModel>();
            containerRegistry.RegisterForNavigation<CSectionView, CSectionViewModel>();
        }

        public void OnInitialized(IContainerProvider containerProvider) {
        }
    }
}