
using System.Windows;
using SAP2000v1;

namespace Sap2000ToolBox {
    public class cPlugin {
        public void Main(ref cSapModel       sapModel,
                         ref cPluginCallback sapPlugin) {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            MessageBox.Show("≥…π¶º”‘ÿ");
            sapPlugin.Finish(0);

            //var x = new MainViewBootStrapper(ref sapModel);

            //x.Run();
        }

        public long Info(ref string text) {
            text = "Author By MissBlue\n";
            return 0;
        }
    }
}
