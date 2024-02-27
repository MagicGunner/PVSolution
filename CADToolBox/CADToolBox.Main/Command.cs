using System.Runtime.InteropServices.ComTypes;
using System.Windows.Media.Animation;
using Autodesk.AutoCAD.Customization;
using CADToolBox.Main.Functions;
using CADToolBox.Modules.TrackerGA;
using CADToolBox.Modules.TrackerGA.Views;
using CADToolBox.Resource.NameDictionarys;
using CADToolBox.Shared.Models.CADModels.Implement;
using Microsoft.Extensions.DependencyInjection;
using Version = Autodesk.AutoCAD.Customization.Version;

namespace CADToolBox.Main;

public class Command {
    [CommandMethod(nameof(HelloWorld))]
    public void HelloWorld() {
        var res = Assembly.Load("CADToolBox.Resource");
        foreach (var manifestResourceName in res.GetManifestResourceNames()) {
            using var tr = new DBTrans();
            Env.Editor.WriteMessage(manifestResourceName);
        }
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
            } else if (promptEntityResult.Status != PromptStatus.OK) { return; }

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
                var propertyValue = projectInput[value];
                property.SetValue(trackerModel, Convert.ChangeType(propertyValue, property.PropertyType));
            }
        }

        var initPostList = new List<PostModel> { new() { Num = 1 }, new() { Num = 2 } };
        trackerModel.PostList = initPostList;

        TrackerApp.Current.TrackerModel = trackerModel;
        TrackerApp.Current.Run();
    }

#region 截面绘制小工具

    [CommandMethod(nameof(SD))]
    public void SD() {
        using var trans = new DBTrans();

        var sectionKeywordDic = new Dictionary<string, string> {
                                                                   { "W", "美标H型钢(W)" },
                                                                   { "RH", "国标热轧H型钢(RH)" },
                                                                   { "WH", "高频焊H型钢(WH)" },
                                                                   { "CFC", "折弯C型钢(CFC)" },
                                                                   { "RC", "热轧槽钢(RC)" },
                                                                   { "CFL", "折弯角钢(CFL)" },
                                                                   { "RL", "热轧角钢(RL)" },
                                                                   { "CFU", "U型钢(CFU)" }
                                                               };
        var currentDoc = Acaop.DocumentManager.MdiActiveDocument;
        var ed         = currentDoc.Editor;

        var pKeyOpts = new PromptKeywordOptions("\n请选择截面类型") { AllowNone = true };
        foreach (var item in sectionKeywordDic) { pKeyOpts.Keywords.Add(item.Value, item.Key, item.Value); }

        var                                            pKeyRes = ed.GetKeywords(pKeyOpts);
        var                                            pPtOpts = new PromptPointOptions("") { Message = "\n请选择插入点" };

        Dictionary<string, Dictionary<string, string>> sectionPropDic; // 截面属性字典
        Dictionary<string, string>?                    sectionProp;    // 选中的截面属性
        PromptPointResult?                             pPointRes;      // 用户选择的插入点

        PromptStringOptions pStringOpt;

        switch (pKeyRes.StringResult) {
            case "美标H型钢(W)":
                pKeyOpts = new PromptKeywordOptions("\n请选择美标H型钢截面");
                sectionPropDic = GeneralTemplateData.PostSectionMap["W型钢"]
                                                    .Select(item => item)
                                                    .ToList()
                                                    .ToDictionary(item => item.Name, item => item.Props);

                foreach (var item in sectionPropDic) { pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key); }

                pKeyRes     = ed.GetKeywords(pKeyOpts);
                sectionProp = sectionPropDic[pKeyRes.StringResult];
                pPointRes   = ed.GetPoint(pPtOpts);
                CadFunctions.DrawHSteel(trans,
                                        pPointRes.Value,
                                        Convert.ToDouble(sectionProp["H"]),
                                        Convert.ToDouble(sectionProp["B"]),
                                        Convert.ToDouble(sectionProp["tw"]),
                                        Convert.ToDouble(sectionProp["tf"]));
                break;
            case "国标热轧H型钢(RH)":
                pKeyOpts = new PromptKeywordOptions("\n请选择国标热轧H型钢");
                sectionPropDic = GeneralTemplateData.PostSectionMap["热轧H型钢"]
                                                    .Select(item => item)
                                                    .ToList()
                                                    .ToDictionary(item => item.Name, item => item.Props);

                foreach (var item in sectionPropDic) { pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key); }

                pKeyRes     = ed.GetKeywords(pKeyOpts);
                sectionProp = sectionPropDic[pKeyRes.StringResult];
                pPointRes   = ed.GetPoint(pPtOpts);
                CadFunctions.DrawHSteel(trans,
                                        pPointRes.Value,
                                        Convert.ToDouble(sectionProp["H"]),
                                        Convert.ToDouble(sectionProp["B"]),
                                        Convert.ToDouble(sectionProp["tw"]),
                                        Convert.ToDouble(sectionProp["tf"]));
                break;
            case "高频焊H型钢(WH)":
                var whSectionData = new List<double>();
                pStringOpt = new PromptStringOptions("\n请输入总高度");
                whSectionData.Add(Convert.ToDouble(ed.GetString(pStringOpt).StringResult));
                pStringOpt = new PromptStringOptions("\n请输入总宽度");
                whSectionData.Add(Convert.ToDouble(ed.GetString(pStringOpt).StringResult));
                pStringOpt = new PromptStringOptions("\n请输入腹板厚度");
                whSectionData.Add(Convert.ToDouble(ed.GetString(pStringOpt).StringResult));
                pStringOpt = new PromptStringOptions("\n请输入翼板厚度");
                whSectionData.Add(Convert.ToDouble(ed.GetString(pStringOpt).StringResult));
                CadFunctions.DrawHSteel(trans,
                                        ed.GetPoint(pPtOpts).Value,
                                        whSectionData[0],
                                        whSectionData[1],
                                        whSectionData[2],
                                        whSectionData[3]);
                break;
            case "折弯C型钢(CFC)":
                var cfcSectionData = new List<double>();
                pStringOpt = new PromptStringOptions("\n请输入总高度");
                cfcSectionData.Add(Convert.ToDouble(ed.GetString(pStringOpt).StringResult));
                pStringOpt = new PromptStringOptions("\n请输入总宽度");
                cfcSectionData.Add(Convert.ToDouble(ed.GetString(pStringOpt).StringResult));
                pStringOpt = new PromptStringOptions("\n请输入翻边高度");
                cfcSectionData.Add(Convert.ToDouble(ed.GetString(pStringOpt).StringResult));
                pStringOpt = new PromptStringOptions("\n请输入厚度");
                cfcSectionData.Add(Convert.ToDouble(ed.GetString(pStringOpt).StringResult));
                pStringOpt = new PromptStringOptions("\n请输入内R角");
                cfcSectionData.Add(Convert.ToDouble(ed.GetString(pStringOpt).StringResult));
                CadFunctions.DrawCSteel(trans,
                                        ed.GetPoint(pPtOpts).Value,
                                        cfcSectionData[0],
                                        cfcSectionData[1],
                                        cfcSectionData[2],
                                        cfcSectionData[3],
                                        cfcSectionData[4]);
                break;
            case "折弯角钢(CFL)": break;
        }
    }

#endregion
}