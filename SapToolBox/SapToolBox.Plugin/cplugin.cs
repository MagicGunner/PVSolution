using SAP2000v1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using SapToolBox.Plugin.Views;

namespace SapToolBox.Plugin {
    public class cPlugin {
        public void Main(ref cSapModel sapModel, ref cPluginCallback sapPlugin) {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            sapPlugin.Finish(0);

            var x = new MainViewBootStrapper(ref sapModel);

            x.Run();
        }

        public long Info(ref string text) {
            text = "Author By MissBlue\n";
            return 0;
        }

        //public Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
        //    var name = args.Name.Split(',')[0];
        //    var assembly = Assembly.GetAssembly(typeof(MainView));
        //    var location = assembly.Location;
        //    var dirPath = System.IO.Path.GetDirectoryName(location);
        //    var dirInfo = new DirectoryInfo(dirPath);
        //    foreach (var item in dirInfo.GetFiles()) {
        //        if (System.IO.Path.GetFileNameWithoutExtension(item.FullName) == name) {
        //            return Assembly.LoadFile(item.FullName);
        //        }
        //    }

        //    return null;
        //}
    }
}
