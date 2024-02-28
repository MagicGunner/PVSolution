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

        var    pKeyRes    = ed.GetKeywords(pKeyOpts);
        var    pPtOpts    = new PromptPointOptions("") { Message = "\n请选择插入点" };
        double pDoubleRes = 0.0;

        Dictionary<string, Dictionary<string, string>> sectionPropDic; // 截面属性字典
        Dictionary<string, string>?                    sectionProp;    // 选中的截面属性
        PromptPointResult?                             pPointRes;      // 用户选择的插入点

        PromptStringOptions pStringOpt;

        switch (pKeyRes.StringResult) {
            case "美标H型钢(W)":
                pKeyOpts       = new PromptKeywordOptions("\n请选择美标H型钢截面");
                sectionPropDic = GeneralTemplateData.WSectionPropDic;

                foreach (var item in sectionPropDic) { pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key); }

                pKeyRes     = ed.GetKeywords(pKeyOpts);
                sectionProp = sectionPropDic[pKeyRes.StringResult];
                pPointRes   = ed.GetPoint(pPtOpts);
                CadFunctions.DrawHSteel(trans,
                                        pPointRes.Value,
                                        Convert.ToDouble(sectionProp["H"]),
                                        Convert.ToDouble(sectionProp["B"]),
                                        Convert.ToDouble(sectionProp["tw"]),
                                        Convert.ToDouble(sectionProp["tf"]),
                                        0);
                break;
            case "国标热轧H型钢(RH)":
                pKeyOpts = new PromptKeywordOptions("\n请选择国标热轧H型钢类型");
                pKeyOpts.Keywords.Add("宽翼缘H型钢(W)", "W", "宽翼缘H型钢(W)");
                pKeyOpts.Keywords.Add("中翼缘H型钢(M)", "M", "中翼缘H型钢(M)");
                pKeyOpts.Keywords.Add("窄翼缘H型钢(N)", "N", "窄翼缘H型钢(N)");
                pKeyOpts.Keywords.Add("薄壁H型钢(T)",  "T", "薄壁H型钢(T)");
                pKeyRes = ed.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.OK) { return; }

                switch (pKeyRes.StringResult) {
                    case "宽翼缘H型钢(W)":
                        pKeyOpts       = new PromptKeywordOptions("\n请选择H型钢截面");
                        sectionPropDic = GeneralTemplateData.RollHWSectionPropDic;

                        foreach (var item in sectionPropDic) { pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key); }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) { return; }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        pPointRes   = ed.GetPoint(pPtOpts);
                        CadFunctions.DrawHSteel(trans,
                                                pPointRes.Value,
                                                Convert.ToDouble(sectionProp["H"]),
                                                Convert.ToDouble(sectionProp["B"]),
                                                Convert.ToDouble(sectionProp["tw"]),
                                                Convert.ToDouble(sectionProp["tf"]),
                                                Convert.ToDouble(sectionProp["r"]));
                        break;
                    case "中翼缘H型钢(M)":
                        pKeyOpts       = new PromptKeywordOptions("\n请选择H型钢截面");
                        sectionPropDic = GeneralTemplateData.RollHMSectionPropDic;

                        foreach (var item in sectionPropDic) { pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key); }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) { return; }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        pPointRes   = ed.GetPoint(pPtOpts);
                        CadFunctions.DrawHSteel(trans,
                                                pPointRes.Value,
                                                Convert.ToDouble(sectionProp["H"]),
                                                Convert.ToDouble(sectionProp["B"]),
                                                Convert.ToDouble(sectionProp["tw"]),
                                                Convert.ToDouble(sectionProp["tf"]),
                                                Convert.ToDouble(sectionProp["r"]));
                        break;
                    case "窄翼缘H型钢(N)":
                        pKeyOpts       = new PromptKeywordOptions("\n请选择H型钢截面");
                        sectionPropDic = GeneralTemplateData.RollHNSectionPropDic;

                        foreach (var item in sectionPropDic) { pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key); }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) { return; }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        pPointRes   = ed.GetPoint(pPtOpts);
                        CadFunctions.DrawHSteel(trans,
                                                pPointRes.Value,
                                                Convert.ToDouble(sectionProp["H"]),
                                                Convert.ToDouble(sectionProp["B"]),
                                                Convert.ToDouble(sectionProp["tw"]),
                                                Convert.ToDouble(sectionProp["tf"]),
                                                Convert.ToDouble(sectionProp["r"]));
                        break;
                    case "薄壁H型钢(T)":
                        pKeyOpts       = new PromptKeywordOptions("\n请选择H型钢截面");
                        sectionPropDic = GeneralTemplateData.RollHTSectionPropDic;

                        foreach (var item in sectionPropDic) { pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key); }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) { return; }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        pPointRes   = ed.GetPoint(pPtOpts);
                        CadFunctions.DrawHSteel(trans,
                                                pPointRes.Value,
                                                Convert.ToDouble(sectionProp["H"]),
                                                Convert.ToDouble(sectionProp["B"]),
                                                Convert.ToDouble(sectionProp["tw"]),
                                                Convert.ToDouble(sectionProp["tf"]),
                                                Convert.ToDouble(sectionProp["r"]));
                        break;
                }


                break;
            case "热轧槽钢(RC)":
                pKeyOpts       = new PromptKeywordOptions("\n请选择国标热轧槽钢");
                sectionPropDic = GeneralTemplateData.RollCSectionPropDic;

                foreach (var item in sectionPropDic) { pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key); }

                pKeyRes = ed.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.OK) { return; }

                sectionProp = sectionPropDic[pKeyRes.StringResult];
                pPointRes   = ed.GetPoint(pPtOpts);
                CadFunctions.DrawRollCSteel(trans,
                                            pPointRes.Value,
                                            Convert.ToDouble(sectionProp["H"]),
                                            Convert.ToDouble(sectionProp["B"]),
                                            Convert.ToDouble(sectionProp["d"]),
                                            Convert.ToDouble(sectionProp["t"]),
                                            Convert.ToDouble(sectionProp["r"]),
                                            Convert.ToDouble(sectionProp["r1"]));
                break;
            case "热轧角钢(RL)":
                pKeyOpts = new PromptKeywordOptions("\n是否等边");
                pKeyOpts.Keywords.Add("Y", "Y", "是(Y)");
                pKeyOpts.Keywords.Add("N", "N", "否(Y)");
                pKeyRes = ed.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.OK) { return; }

                switch (pKeyRes.StringResult) {
                    case "Y":
                        pKeyOpts       = new PromptKeywordOptions("\n请选择等边热轧角钢");
                        sectionPropDic = GeneralTemplateData.RollEqualLSectionPropDic;

                        foreach (var item in sectionPropDic) { pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key); }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) { return; }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        pPointRes   = ed.GetPoint(pPtOpts);
                        CadFunctions.DrawRollLSteel(trans,
                                                    pPointRes.Value,
                                                    Convert.ToDouble(sectionProp["b"]),
                                                    Convert.ToDouble(sectionProp["b"]),
                                                    Convert.ToDouble(sectionProp["d"]),
                                                    Convert.ToDouble(sectionProp["r"]));
                        break;
                    case "N":
                        pKeyOpts       = new PromptKeywordOptions("\n请选择不等边热轧角钢");
                        sectionPropDic = GeneralTemplateData.RollUnEqualLSectionPropDic;

                        foreach (var item in sectionPropDic) { pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key); }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) { return; }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        pPointRes   = ed.GetPoint(pPtOpts);
                        CadFunctions.DrawRollLSteel(trans,
                                                    pPointRes.Value,
                                                    Convert.ToDouble(sectionProp["B"]),
                                                    Convert.ToDouble(sectionProp["b"]),
                                                    Convert.ToDouble(sectionProp["d"]),
                                                    Convert.ToDouble(sectionProp["r"]));
                        break;
                }

                break;
            case "高频焊H型钢(WH)":
                var whSectionData = new List<double>();
                pStringOpt = new PromptStringOptions("\n请输入总高度");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                whSectionData.Add(pDoubleRes);
                pStringOpt = new PromptStringOptions("\n请输入总宽度");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                whSectionData.Add(pDoubleRes);
                pStringOpt = new PromptStringOptions("\n请输入腹板厚度");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                whSectionData.Add(pDoubleRes);
                pStringOpt = new PromptStringOptions("\n请输入翼板厚度");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                whSectionData.Add(pDoubleRes);
                CadFunctions.DrawHSteel(trans,
                                        ed.GetPoint(pPtOpts).Value,
                                        whSectionData[0],
                                        whSectionData[1],
                                        whSectionData[2],
                                        whSectionData[3],
                                        0);
                break;
            case "折弯C型钢(CFC)":
                var cfcSectionData = new List<double>();
                pStringOpt = new PromptStringOptions("\n请输入总高度");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                cfcSectionData.Add(pDoubleRes);
                pStringOpt = new PromptStringOptions("\n请输入总宽度");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                cfcSectionData.Add(pDoubleRes);
                pStringOpt = new PromptStringOptions("\n请输入翻边高度");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                cfcSectionData.Add(pDoubleRes);
                pStringOpt = new PromptStringOptions("\n请输入厚度");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                cfcSectionData.Add(pDoubleRes);
                pStringOpt = new PromptStringOptions("\n请输入内R角");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                cfcSectionData.Add(pDoubleRes);
                CadFunctions.DrawCSteel(trans,
                                        ed.GetPoint(pPtOpts).Value,
                                        cfcSectionData[0],
                                        cfcSectionData[1],
                                        cfcSectionData[2],
                                        cfcSectionData[3],
                                        cfcSectionData[4]);
                break;
            case "折弯角钢(CFL)":
                var cflSectionData = new List<double>();
                pStringOpt = new PromptStringOptions("\n请输入单肢高度(竖直)");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                cflSectionData.Add(pDoubleRes);
                pStringOpt = new PromptStringOptions("\n请输入单肢高度(水平)");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                cflSectionData.Add(pDoubleRes);
                pStringOpt = new PromptStringOptions("\n请输入厚度");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                cflSectionData.Add(pDoubleRes);
                pStringOpt = new PromptStringOptions("\n请输入内R角");
                while (true) {
                    if (double.TryParse(ed.GetString(pStringOpt).StringResult, out pDoubleRes)) break;
                }

                cflSectionData.Add(pDoubleRes);
                CadFunctions.DrawLSteel(trans,
                                        ed.GetPoint(pPtOpts).Value,
                                        cflSectionData[0],
                                        cflSectionData[1],
                                        cflSectionData[2],
                                        cflSectionData[3]);
                break;
        }
    }

#endregion
}