using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using System;

namespace load {
    public class Class1 {
        private bool ev = false;

        [CommandMethod("ww")]
        public void ww() {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db  = doc.Database;
            var ed  = doc.Editor;
            var ad
                = new
                    AssemblyDependent(@"E:\00-Code\PVSolution\CADToolBox\CADToolBox.Main\bin\Debug\net48\CADToolBox.Main.dll"); //写上你dll的路径
            var msg = ad.Load();

            bool allyes = true;
            foreach (var item in msg) {
                if (!item.LoadYes) {
                    ed.WriteMessage(Environment.NewLine           + "**" + item.Path + Environment.NewLine +
                                    "**此文件已加载过,重复名称,重复版本号,本次不加载!" + Environment.NewLine);
                    allyes = false;
                }
            }

            if (allyes) {
                ed.WriteMessage(Environment.NewLine + "**链式加载成功!" + Environment.NewLine);
            }

            if (!ev) {
                AppDomain.CurrentDomain.AssemblyResolve += RunTimeCurrentDomain.DefaultAssemblyResolve;
                ev                                      =  true;
            }

            // 加载跟踪支架界面模块
            ad = new
                AssemblyDependent(@"E:\00-Code\PVSolution\CADToolBox\CADToolBox.Main\bin\Debug\net48\CADToolBox.Modules.TrackerGA.dll");
            msg    = ad.Load();
            allyes = true;
            foreach (var item in msg) {
                if (!item.LoadYes) {
                    ed.WriteMessage(Environment.NewLine           + "**" + item.Path + Environment.NewLine +
                                    "**此文件已加载过,重复名称,重复版本号,本次不加载!" + Environment.NewLine);
                    allyes = false;
                }
            }

            if (allyes) {
                ed.WriteMessage(Environment.NewLine + "**链式加载成功!" + Environment.NewLine);
            }

            if (!ev) {
                AppDomain.CurrentDomain.AssemblyResolve += RunTimeCurrentDomain.DefaultAssemblyResolve;
                ev                                      =  true;
            }

            // 加载固定支架界面模块
            ad = new AssemblyDependent(@"E:\00-Code\PVSolution\CADToolBox\CADToolBox.Main\bin\Debug\net48\CADToolBox.Modules.FixedGA.dll");
            msg = ad.Load();
            allyes = true;
            foreach (var item in msg) {
                if (!item.LoadYes) {
                    ed.WriteMessage(Environment.NewLine + "**" + item.Path + Environment.NewLine + "**此文件已加载过,重复名称,重复版本号,本次不加载!" + Environment.NewLine);
                    allyes = false;
                }
            }

            if (allyes) {
                ed.WriteMessage(Environment.NewLine + "**链式加载成功!" + Environment.NewLine);
            }

            if (!ev) {
                AppDomain.CurrentDomain.AssemblyResolve += RunTimeCurrentDomain.DefaultAssemblyResolve;
                ev = true;
            }


            // 加载资源模块
            ad = new
                AssemblyDependent(@"E:\00-Code\PVSolution\CADToolBox\CADToolBox.Main\bin\Debug\net48\CADToolBox.Resource.dll");
            msg    = ad.Load();
            allyes = true;
            foreach (var item in msg) {
                if (!item.LoadYes) {
                    ed.WriteMessage(Environment.NewLine           + "**" + item.Path + Environment.NewLine +
                                    "**此文件已加载过,重复名称,重复版本号,本次不加载!" + Environment.NewLine);
                    allyes = false;
                }
            }

            if (allyes) {
                ed.WriteMessage(Environment.NewLine + "**链式加载成功!" + Environment.NewLine);
            }

            if (!ev) {
                AppDomain.CurrentDomain.AssemblyResolve += RunTimeCurrentDomain.DefaultAssemblyResolve;
                ev                                      =  true;
            }

            // 加载共享
            ad = new
                AssemblyDependent(@"E:\00-Code\PVSolution\CADToolBox\CADToolBox.Main\bin\Debug\net48\CADToolBox.Shared.dll");
            msg    = ad.Load();
            allyes = true;
            foreach (var item in msg) {
                if (!item.LoadYes) {
                    ed.WriteMessage(Environment.NewLine           + "**" + item.Path + Environment.NewLine +
                                    "**此文件已加载过,重复名称,重复版本号,本次不加载!" + Environment.NewLine);
                    allyes = false;
                }
            }

            if (allyes) {
                ed.WriteMessage(Environment.NewLine + "**链式加载成功!" + Environment.NewLine);
            }

            if (!ev) {
                AppDomain.CurrentDomain.AssemblyResolve += RunTimeCurrentDomain.DefaultAssemblyResolve;
                ev                                      =  true;
            }
        }
    }
}