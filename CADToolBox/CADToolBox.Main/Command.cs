using Autodesk.AutoCAD.Customization;
using CADToolBox.Modules.TrackerGA;
using CADToolBox.Modules.TrackerGA.Views;
using CADToolBox.Resource.NameDictionarys;
using CADToolBox.Shared.Models.CADModels.Implement;
using Microsoft.Extensions.DependencyInjection;

namespace CADToolBox.Main;

public class Command {
    [CommandMethod(nameof(HelloWorld))]
    public void HelloWorld() {
        using var tr = new DBTrans();
        Env.Editor.WriteMessage("Hello 333!");
    }

    [CommandMethod(nameof(TestWpf))]
    public void TestWpf() {
        var trackerModel = new TrackerModel();


        var currentDoc = Acaop.DocumentManager.MdiActiveDocument;
        var currentDB  = currentDoc.Database;
        var ed         = currentDoc.Editor;

        var promptEntityOptions = new PromptEntityOptions("\n请选择设计输入增强属性块") { AllowNone = true };
        promptEntityOptions.Keywords.Add("N", "N", "新建(N)");
        PromptEntityResult promptEntityResult;
        while (true) {
            promptEntityResult = ed.GetEntity(promptEntityOptions);
            if (promptEntityResult.Status == PromptStatus.Keyword) {
                if (promptEntityResult.StringResult.ToUpper() == "N") {
                    TrackerApp.Current.Run();
                    break;
                }
            } else if (promptEntityResult.Status != PromptStatus.OK) {
                return;
            }

            break;
        }

        using var tr = new DBTrans();
        if (promptEntityResult.ObjectId != ObjectId.Null) {
            var projectData         = tr.GetObject(promptEntityResult.ObjectId) as BlockReference;
            var attributeCollection = projectData?.AttributeCollection;
            var projectInput        = new Dictionary<string, string>();
            if (attributeCollection != null)
                foreach (ObjectId attributeId in attributeCollection) {
                    var attributeReference = tr.GetObject(attributeId) as AttributeReference;
                    if (attributeReference == null) continue;
                    projectInput.Add(attributeReference.Tag, attributeReference.TextString);
                }

            var properties = typeof(TrackerModel).GetProperties();
            foreach (var property in properties) {
                if (!CadNameDictionarys.AttrNameDic.TryGetValue(property.Name, out var value)) continue;
                if (!projectInput.ContainsKey(value)) continue;
                var propertyValue     = projectInput[value];
                property.SetValue(trackerModel, Convert.ChangeType(propertyValue, property.PropertyType));
            }
        }

        TrackerApp.Current.TrackerModel = trackerModel;
        TrackerApp.Current.Run();
    }
}