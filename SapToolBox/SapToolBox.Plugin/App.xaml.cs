using Prism.DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prism.Ioc;
using Prism.Modularity;
using SAP2000v1;
using SapToolBox.Base.Sap2000;
using SapToolBox.Plugin.Common;
using SapToolBox.Plugin.ViewModels;
using SapToolBox.Plugin.Views;
using SapToolBox.PreTools;
using SapToolBox.PreTools.ViewModels;
using SapToolBox.PreTools.Views;

namespace SapToolBox.Plugin {
    public partial class App : PrismApplication {
        public SapModelHelper SapModelHelper;

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            cHelper myHelper  = new Helper();
            var     sapObject = myHelper.GetObject("CSI.SAP2000.API.SapObject");
            var     sapModel  = sapObject.SapModel;
            SapModelHelper = new SapModelHelper(ref sapModel);
            containerRegistry.RegisterInstance(SapModelHelper);
            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();
            containerRegistry.RegisterForNavigation<PreIndexView, PreIndexViewModel>();
        }

        protected override Window CreateShell() {
            return Container.Resolve<MainView>();
        }

        protected override void OnInitialized() {
            var service = App.Current.MainWindow.DataContext as IConfigureService;
            if (service != null) {
                service.Configure();
            }

            base.OnInitialized();
        }

        /// <summary>
        /// 模块加载
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
            moduleCatalog.AddModule<PreToolsModule>();
        }
    }
}