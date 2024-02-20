using Prism.Ioc;
using Prism.Modularity;
using SapToolBox.PreTools.ViewModels;
using SapToolBox.PreTools.ViewModels.SubViewModels;
using SapToolBox.PreTools.ViewModels.SubViewModels.SectionViewModels;
using SapToolBox.PreTools.Views;
using SapToolBox.PreTools.Views.SubViews;
using SapToolBox.PreTools.Views.SubViews.SectionViews;

namespace SapToolBox.PreTools {
    public class PreToolsModule : IModule {
        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterSingleton<PreIndexViewModel>();

            containerRegistry.RegisterForNavigation<PreIndexView, PreIndexViewModel>();
            containerRegistry.RegisterForNavigation<DesignOverwriteView, DesignOverwriteViewModel>();
            containerRegistry.RegisterForNavigation<ModelSupportView, ModelSupportViewModel>();

            containerRegistry.RegisterForNavigation<SectionDesignerView, SectionDesignerViewModel>();
            containerRegistry.RegisterForNavigation<HSectionView, HSectionViewModel>();
            containerRegistry.RegisterForNavigation<CSectionView, CSectionViewModel>();
        }

        public void OnInitialized(IContainerProvider containerProvider) {
        }
    }
}