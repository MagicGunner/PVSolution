using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using SAP2000v1;
using SapToolBox.Main.Common.Interface;
using SapToolBox.Main.Views;
using SapToolBox.Modules.CommonTools;
using SapToolBox.Modules.CommonTools.ViewModels;
using SapToolBox.Modules.CommonTools.Views;
using SapToolBox.Modules.DesignTools;
using SapToolBox.Modules.DesignTools.ViewModels;
using SapToolBox.Modules.DesignTools.Views;
using SapToolBox.Shared.Helpers;

namespace SapToolBox.Main {
    public partial class App : PrismApplication {
        //public SapModelHelper SapModelHelper;

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            //cHelper myHelper  = new Helper();
            //var     sapObject = myHelper.GetObject("CSI.SAP2000.API.SapObject");
            //var     sapModel  = sapObject?.SapModel;
            //SapModelHelper = new SapModelHelper();
            //containerRegistry.RegisterInstance(SapModelHelper);
            containerRegistry.RegisterForNavigation<DesignToolsIndexView, DesignToolsIndexViewModel>();
            containerRegistry.RegisterForNavigation<CommonToolsIndexView, CommonToolsIndexViewModel>();
        }

        protected override Window CreateShell() {
            return Container.Resolve<MainView>();
        }

        protected override void OnInitialized() {
            if (Current.MainWindow?.DataContext is IConfigureService service) { service.Configure(); }

            base.OnInitialized();
        }

        /// <summary>
        /// 模块加载
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
            moduleCatalog.AddModule<DesignToolsModule>();
            moduleCatalog.AddModule<CommonToolsModule>();
        }
    }
}