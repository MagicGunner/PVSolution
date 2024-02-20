using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SAP2000v1;
using SapToolBox.Base;
using SapToolBox.Base.Sap2000;
using SapToolBox.Plugin.Common;
using SapToolBox.Plugin.ViewModels;
using SapToolBox.Plugin.Views;
using SapToolBox.PreTools;

namespace SapToolBox.Plugin {
    public class MainViewBootStrapper : PrismBootstrapper {
        private readonly SapModelHelper _sapModelHelper;

        public MainViewBootStrapper(ref cSapModel sapModel) {
            _sapModelHelper = new SapModelHelper(ref sapModel);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            // 依赖注入sap模型对象，以便在窗口中获取
            containerRegistry.RegisterInstance(_sapModelHelper);
            //containerRegistry.RegisterForNavigation<MainView>();
        }

        protected override DependencyObject CreateShell() {
            return Container.Resolve<MainView>();
        }

        protected override void OnInitialized() {
            var service = App.Current.MainWindow.DataContext as IConfigureService;
            if (service != null) {
                service.Configure();
            }

            //var regionManager = Container.Resolve<IRegionManager>();
            //regionManager.RequestNavigate(PrismManager.HomeViewRegionName, "MainView");
            base.OnInitialized();
        }

        /// <summary>
        /// 模块加载
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
            moduleCatalog.AddModule<PreToolsModule>();
        }

        //protected override void InitializeShell(DependencyObject shell) {
        //    Container.Resolve<MainView>().Show();
        //}
    }
}