using System.Runtime.InteropServices.ComTypes;
using System.Windows.Media.Animation;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.EditorInput;
using CADToolBox.Main.Functions;
using CADToolBox.Main.GAHelper;
using CADToolBox.Modules.FixedGA;
using CADToolBox.Modules.TrackerGA;
using CADToolBox.Modules.TrackerGA.Views;
using CADToolBox.Resource.NameDictionarys;
using CADToolBox.Shared.Models.CADModels.Implement;
using CADToolBox.Shared.Models.CADModels.Implement.Tracker;
using IFoxCAD.Cad;
using Microsoft.Extensions.DependencyInjection;
using PromptKeywordOptions = Autodesk.AutoCAD.EditorInput.PromptKeywordOptions;
using Version = Autodesk.AutoCAD.Customization.Version;

namespace CADToolBox.Main;

public class Command {
    #region 测试用

    [CommandMethod(nameof(ChangeBlocksName))]
    public void ChangeBlocksName() {
        CadFunctions.UpdateBlockName(new DBTrans(), "AntaiGA", "Linsum");
    }

    [CommandMethod(nameof(InsertBlock))]
    public void InsertBlock() {
        var trans = new DBTrans();
        var dwgFileName = @"E:\00-Code\PVSolution\CADToolBox\CADToolBox.Resource\Template\GA-template.dwg";
        CadFunctions.ImportBlockFromDwg(trans, dwgFileName);
        CadFunctions.UpdateBlockName(trans, "Linsum", "Linsum-跟踪支架");
        dwgFileName = @"E:\00-Code\PVSolution\CADToolBox\CADToolBox.Resource\Template\固定支架GA图模板.dwg";
        CadFunctions.ImportBlockFromDwg(trans, dwgFileName);
        CadFunctions.UpdateBlockName(trans, "AntaiGA", "Linsum-固定支架");
    }

    #endregion

    #region 固定支架排布图

    [CommandMethod(nameof(FixedGA))]
    public void FixedGA() {
        FixedApp.Current.Run();
    }

    #endregion

    #region 跟踪支架GA图辅助截面

    [CommandMethod(nameof(TrackerGA))]
    public void TrackerGA() {
        var companyName = "Linsum";
        var trackerModel = new TrackerModel();
        var currentDoc = Acaop.DocumentManager.MdiActiveDocument;
        var ed = currentDoc.Editor;

        using var trans = new DBTrans();

        var pEntityOpts = new PromptEntityOptions("\n请选择设计输入增强属性块") { AllowNone = true };
        pEntityOpts.Keywords.Add("新建(N)", "N", "新建(N)");
        var pEntityRes = ed.GetEntity(pEntityOpts);
        switch (pEntityRes.Status) {
            case PromptStatus.Keyword: {
                if (pEntityRes.StringResult == "新建(N)") { // 新建项目的情况
                    trackerModel = new TrackerModel();
                    var initPostList = new List<PostModel> {
                                                               new() { Num = 1 },
                                                               new() { Num = 2 }
                                                           };
                    var initBeamList = new List<BeamModel> {
                                                               new() { Num = 1 },
                                                               new() { Num = 2 }
                                                           };
                    trackerModel.PostList = initPostList;
                    trackerModel.BeamList = initBeamList;
                }

                break;
            }
            case PromptStatus.OK: {
                if (pEntityRes.ObjectId == ObjectId.Null) return;
                var projectData = trans.GetObject(pEntityRes.ObjectId) as BlockReference;
                if (projectData == null || projectData.Name != "00-" + companyName + "-跟踪支架-国标输入") return;

                var attributeCollection = projectData.AttributeCollection;
                var projectInput = new Dictionary<string, string>();
                if (attributeCollection != null)
                    foreach (ObjectId attributeId in attributeCollection) {
                        var attributeReference = trans.GetObject(attributeId) as AttributeReference;
                        if (attributeReference == null) continue;
                        projectInput.Add(attributeReference.Tag, attributeReference.TextString);
                    }

                // 初始化TrackerModel中的属性(除去立柱列表与主梁列表)
                var properties = typeof(TrackerModel).GetProperties();
                foreach (var property in properties) {
                    if (!CadNameDictionarys.AttrNameDic.TryGetValue(property.Name, out var value)) continue;
                    if (!projectInput.ContainsKey(value)) continue;
                    var propertyValue = projectInput[value];
                    property.SetValue(trackerModel, Convert.ChangeType(propertyValue, property.PropertyType));
                }

                // 初始化立柱数组
                trackerModel.PostList = new List<PostModel> { Capacity = 0 };
                var postNumList = projectInput["立柱序号"].Split(' ');
                var postType = projectInput["立柱类型"].Split(' ');
                var postSectionType = projectInput["立柱截面类型"].Split(' ');
                var postSection = projectInput["立柱截面规格"].Split(' ');
                var postMaterial = projectInput["立柱截面材质"].Split(' ');
                var postLeftSpan = projectInput["左侧跨距"].Split(' ');
                var postRightSpan = projectInput["右侧跨距"].Split(' ');
                var postLeftToBeam = projectInput["左侧开断"].Split(' ');
                var postRightToBeam = projectInput["右侧开断"].Split(' ');
                var postPileUpGround = projectInput["基础露头"].Split(' ');
                var postPileDownGround = projectInput["基础埋深"].Split(' ');
                for (var i = 0; i < postNumList.Length; i++) {
                    var newPostModel = new PostModel {
                                                         Num = i + 1,
                                                         IsDrive = postType[i] != "普通立柱",
                                                         SectionType = postSectionType[i],
                                                         Section = postSection[i],
                                                         Material = postMaterial[i],
                                                     };
                    if (double.TryParse(postLeftSpan[i], out var tempValue)) newPostModel.LeftSpan = tempValue;
                    if (double.TryParse(postRightSpan[i], out tempValue)) newPostModel.RightSpan = tempValue;
                    if (double.TryParse(postLeftToBeam[i], out tempValue)) newPostModel.LeftToBeam = tempValue;
                    if (double.TryParse(postRightToBeam[i], out tempValue)) newPostModel.RightToBeam = tempValue;
                    if (double.TryParse(postPileUpGround[i], out tempValue)) newPostModel.PileUpGround = tempValue;
                    if (double.TryParse(postPileDownGround[i], out tempValue)) newPostModel.PileDownGround = tempValue;
                    trackerModel.PostList.Add(newPostModel);
                }

                // 初始化主梁数组
                trackerModel.BeamList = new List<BeamModel> { Capacity = 0 };
                var beamNumList = projectInput["主梁序号"].Split(' ');
                var beamSectionType = projectInput["主梁截面类型"].Split(' ');
                var beamSection = projectInput["主梁截面规格"].Split(' ');
                var beamMaterial = projectInput["主梁截面材质"].Split(' ');
                var beamLength = projectInput["分段长度"].Split(' ');
                var leftToBeam = projectInput["到上一段距离"].Split(' ');
                var rightToBeam = projectInput["到下一段距离"].Split(' ');
                for (var i = 0; i < beamNumList.Length; i++) {
                    var newBeamModel = new BeamModel {
                                                         Num = i + 1,
                                                         SectionType = beamSectionType[i],
                                                         Section = beamSection[i],
                                                         Material = beamMaterial[i],
                                                         LeftToPre = Convert.ToDouble(leftToBeam[i]),
                                                         RightToNext = Convert.ToDouble(rightToBeam[i])
                                                     };
                    if (double.TryParse(beamLength[i], out var tempValue)) newBeamModel.Length = tempValue;
                    trackerModel.BeamList.Add(newBeamModel);
                }

                break;
            } // 选择已有的项目
            case PromptStatus.Cancel:   break;
            case PromptStatus.None:     break;
            case PromptStatus.Error:    break;
            case PromptStatus.Modeless: break;
            case PromptStatus.Other:    break;
            default:                    throw new ArgumentOutOfRangeException();
        }

        trackerModel.Init();
        TrackerApp.Current.TrackerModel = trackerModel;
        TrackerApp.Current.Run();


        if (trackerModel.Status == -1) return;

        PromptKeywordOptions pKeyOpts;
        PromptResult pKeyRes;

        var pPointOpts = new PromptPointOptions("\n请选择插入点");
        var insertPoint = Point3d.Origin;
        switch (trackerModel.Status) {
            case 0: // 仅保存
                pKeyOpts = new PromptKeywordOptions("是否新建项目");
                pKeyOpts.Keywords.Add("Y", "Y", "新建项目(Y)");
                pKeyOpts.Keywords.Add("N", "N", "保存至原来项目(N)");
                pKeyRes = ed.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.OK) return;
                switch (pKeyRes.StringResult) {
                    case "Y":
                        CadFunctions.WriteToInput(trans, ed.GetPoint(pPointOpts).Value, trackerModel, new Scale3d(), 0, "00-" + companyName + "-跟踪支架-国标输入");
                        break;
                    case "N":
                        pEntityOpts = new PromptEntityOptions("\n请选择现有的信息输入块");
                        pEntityRes = ed.GetEntity(pEntityOpts);
                        if (pEntityRes.Status == PromptStatus.OK) {
                            var inputData = trans.GetObject(pEntityRes.ObjectId) as BlockReference;
                            if (inputData != null) CadFunctions.SaveToInput(trans, inputData, trackerModel);
                        }

                        break;
                }

                return;
            case 1: // 仅绘图
                insertPoint = ed.GetPoint(pPointOpts).Value;
                break;
            case 2: // 保存并绘图
                pKeyOpts = new PromptKeywordOptions("是否新建项目");
                pKeyOpts.Keywords.Add("Y", "Y", "新建项目(Y)");
                pKeyOpts.Keywords.Add("N", "N", "保存至原来项目(N)");
                pKeyRes = ed.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.OK) return;
                switch (pKeyRes.StringResult) {
                    case "Y":
                        CadFunctions.WriteToInput(trans, ed.GetPoint(pPointOpts).Value, trackerModel, new Scale3d(), 0, "00-" + companyName + "-跟踪支架-国标输入");
                        // 绘图代码********************************************************************
                        break;
                    case "N":
                        pEntityOpts = new PromptEntityOptions("\n请选择现有的信息输入块");
                        pEntityRes = ed.GetEntity(pEntityOpts);
                        if (pEntityRes.Status == PromptStatus.OK) {
                            var inputData = trans.GetObject(pEntityRes.ObjectId) as BlockReference;
                            if (inputData != null) CadFunctions.SaveToInput(trans, inputData, trackerModel);
                        }
                        // 绘图代码********************************************************************

                        break;
                }

                insertPoint = ed.GetPoint(pPointOpts).Value;
                break;
        }

        var trackerGAHelper = new TrackerGAHelper(trackerModel, trans, insertPoint);
        trackerGAHelper.InitStyles();
        trackerGAHelper.GetGA();
    }

    #endregion


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
                                                                   { "CFU", "U型钢(CFU)" },
                                                                   { "P", "圆管(P)" },
                                                                   { "ST", "矩形管(ST)" }
                                                               };
        var currentDoc = Acaop.DocumentManager.MdiActiveDocument;
        var ed = currentDoc.Editor;

        var pKeyOpts = new PromptKeywordOptions("\n请选择截面类型") { AllowNone = true };
        foreach (var item in sectionKeywordDic) {
            pKeyOpts.Keywords.Add(item.Value, item.Key, item.Value);
        }

        var pKeyRes = ed.GetKeywords(pKeyOpts);
        var pPointOpt = new PromptPointOptions("") { Message = "\n请选择插入点" };

        Dictionary<string, Dictionary<string, string>> sectionPropDic; // 截面属性字典
        Dictionary<string, string>? sectionProp;                       // 选中的截面属性

        PromptDoubleOptions pDoubleOpt;
        PromptStringOptions pStringOpt;

        PromptPointResult? pPointRes;  // 用户选择的插入点
        PromptDoubleResult pDoubleRes; //用书输入浮点数

        switch (pKeyRes.StringResult) {
            case "美标H型钢(W)":
                pKeyOpts = new PromptKeywordOptions("\n请选择美标H型钢截面");
                sectionPropDic = GeneralTemplateData.WSectionPropDic;

                foreach (var item in sectionPropDic) {
                    pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key);
                }

                pKeyRes = ed.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.OK) {
                    return;
                }

                sectionProp = sectionPropDic[pKeyRes.StringResult];
                pPointRes = ed.GetPoint(pPointOpt);
                CadFunctions.DrawHSteel(trans, pPointRes.Value, Convert.ToDouble(sectionProp["H"]), Convert.ToDouble(sectionProp["B"]), Convert.ToDouble(sectionProp["tw"]), Convert.ToDouble(sectionProp["tf"]), 0);
                break;
            case "国标热轧H型钢(RH)":
                pKeyOpts = new PromptKeywordOptions("\n请选择国标热轧H型钢类型");
                pKeyOpts.Keywords.Add("宽翼缘H型钢(W)", "W", "宽翼缘H型钢(W)");
                pKeyOpts.Keywords.Add("中翼缘H型钢(M)", "M", "中翼缘H型钢(M)");
                pKeyOpts.Keywords.Add("窄翼缘H型钢(N)", "N", "窄翼缘H型钢(N)");
                pKeyOpts.Keywords.Add("薄壁H型钢(T)", "T", "薄壁H型钢(T)");
                pKeyRes = ed.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.OK) {
                    return;
                }

                switch (pKeyRes.StringResult) {
                    case "宽翼缘H型钢(W)":
                        pKeyOpts = new PromptKeywordOptions("\n请选择H型钢截面");
                        sectionPropDic = GeneralTemplateData.RollHWSectionPropDic;

                        foreach (var item in sectionPropDic) {
                            pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key);
                        }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) {
                            return;
                        }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        pPointRes = ed.GetPoint(pPointOpt);
                        CadFunctions.DrawHSteel(trans, pPointRes.Value, Convert.ToDouble(sectionProp["H"]), Convert.ToDouble(sectionProp["B"]), Convert.ToDouble(sectionProp["tw"]), Convert.ToDouble(sectionProp["tf"]), Convert.ToDouble(sectionProp["r"]));
                        break;
                    case "中翼缘H型钢(M)":
                        pKeyOpts = new PromptKeywordOptions("\n请选择H型钢截面");
                        sectionPropDic = GeneralTemplateData.RollHMSectionPropDic;

                        foreach (var item in sectionPropDic) {
                            pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key);
                        }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) {
                            return;
                        }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        pPointRes = ed.GetPoint(pPointOpt);
                        CadFunctions.DrawHSteel(trans, pPointRes.Value, Convert.ToDouble(sectionProp["H"]), Convert.ToDouble(sectionProp["B"]), Convert.ToDouble(sectionProp["tw"]), Convert.ToDouble(sectionProp["tf"]), Convert.ToDouble(sectionProp["r"]));
                        break;
                    case "窄翼缘H型钢(N)":
                        pKeyOpts = new PromptKeywordOptions("\n请选择H型钢截面");
                        sectionPropDic = GeneralTemplateData.RollHNSectionPropDic;

                        foreach (var item in sectionPropDic) {
                            pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key);
                        }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) {
                            return;
                        }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        pPointRes = ed.GetPoint(pPointOpt);
                        CadFunctions.DrawHSteel(trans, pPointRes.Value, Convert.ToDouble(sectionProp["H"]), Convert.ToDouble(sectionProp["B"]), Convert.ToDouble(sectionProp["tw"]), Convert.ToDouble(sectionProp["tf"]), Convert.ToDouble(sectionProp["r"]));
                        break;
                    case "薄壁H型钢(T)":
                        pKeyOpts = new PromptKeywordOptions("\n请选择H型钢截面");
                        sectionPropDic = GeneralTemplateData.RollHTSectionPropDic;

                        foreach (var item in sectionPropDic) {
                            pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key);
                        }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) {
                            return;
                        }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        pPointRes = ed.GetPoint(pPointOpt);
                        CadFunctions.DrawHSteel(trans, pPointRes.Value, Convert.ToDouble(sectionProp["H"]), Convert.ToDouble(sectionProp["B"]), Convert.ToDouble(sectionProp["tw"]), Convert.ToDouble(sectionProp["tf"]), Convert.ToDouble(sectionProp["r"]));
                        break;
                }


                break;
            case "热轧槽钢(RC)":
                pKeyOpts = new PromptKeywordOptions("\n请选择国标热轧槽钢");
                sectionPropDic = GeneralTemplateData.RollCSectionPropDic;

                foreach (var item in sectionPropDic) {
                    pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key);
                }

                pKeyRes = ed.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.OK) {
                    return;
                }

                sectionProp = sectionPropDic[pKeyRes.StringResult];
                pPointRes = ed.GetPoint(pPointOpt);
                CadFunctions.DrawRollCSteel(trans, pPointRes.Value, Convert.ToDouble(sectionProp["H"]), Convert.ToDouble(sectionProp["B"]), Convert.ToDouble(sectionProp["d"]), Convert.ToDouble(sectionProp["t"]), Convert.ToDouble(sectionProp["r"]), Convert.ToDouble(sectionProp["r1"]));
                break;
            case "热轧角钢(RL)":
                pKeyOpts = new PromptKeywordOptions("\n是否等边");
                pKeyOpts.Keywords.Add("Y", "Y", "是(Y)");
                pKeyOpts.Keywords.Add("N", "N", "否(N)");
                pKeyRes = ed.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.OK) {
                    return;
                }

                switch (pKeyRes.StringResult) {
                    case "Y":
                        pKeyOpts = new PromptKeywordOptions("\n请选择等边热轧角钢");
                        sectionPropDic = GeneralTemplateData.RollEqualLSectionPropDic;

                        foreach (var item in sectionPropDic) {
                            pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key);
                        }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) {
                            return;
                        }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        pPointRes = ed.GetPoint(pPointOpt);
                        CadFunctions.DrawRollLSteel(trans, pPointRes.Value, Convert.ToDouble(sectionProp["b"]), Convert.ToDouble(sectionProp["b"]), Convert.ToDouble(sectionProp["d"]), Convert.ToDouble(sectionProp["r"]));
                        break;
                    case "N":
                        pKeyOpts = new PromptKeywordOptions("\n请选择不等边热轧角钢");
                        sectionPropDic = GeneralTemplateData.RollUnEqualLSectionPropDic;

                        foreach (var item in sectionPropDic) {
                            pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key);
                        }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) {
                            return;
                        }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        pPointRes = ed.GetPoint(pPointOpt);
                        CadFunctions.DrawRollLSteel(trans, pPointRes.Value, Convert.ToDouble(sectionProp["B"]), Convert.ToDouble(sectionProp["b"]), Convert.ToDouble(sectionProp["d"]), Convert.ToDouble(sectionProp["r"]));
                        break;
                }

                break;
            case "高频焊H型钢(WH)":
                var whSectionData = new List<double>();
                pDoubleOpt = new PromptDoubleOptions("\n请输入总高度");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                whSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入总宽度");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                whSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入腹板厚度");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                whSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入翼缘厚度");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                whSectionData.Add(pDoubleRes.Value);
                CadFunctions.DrawHSteel(trans, ed.GetPoint(pPointOpt).Value, whSectionData[0], whSectionData[1], whSectionData[2], whSectionData[3], 0);
                break;
            case "折弯C型钢(CFC)":
                var cfcSectionData = new List<double>();
                pDoubleOpt = new PromptDoubleOptions("\n请输入总高度");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                cfcSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入总宽度");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                cfcSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入翻边高度");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                cfcSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入厚度");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                cfcSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入内R角");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                cfcSectionData.Add(pDoubleRes.Value);
                CadFunctions.DrawCSteel(trans, ed.GetPoint(pPointOpt).Value, cfcSectionData[0], cfcSectionData[1], cfcSectionData[2], cfcSectionData[3], cfcSectionData[4]);
                break;
            case "折弯角钢(CFL)":
                var cflSectionData = new List<double>();
                pDoubleOpt = new PromptDoubleOptions("\n请输入单肢高度(竖直)");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                cflSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入单肢高度(水平)");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                cflSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入厚度");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                cflSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入内R角");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) return;
                cflSectionData.Add(pDoubleRes.Value);
                CadFunctions.DrawLSteel(trans, ed.GetPoint(pPointOpt).Value, cflSectionData[0], cflSectionData[1], cflSectionData[2], cflSectionData[3]);
                break;
            case "圆管(P)":
                pKeyOpts = new PromptKeywordOptions("\n请选择圆管类型");
                pKeyOpts.Keywords.Add("自定义截面(N)", "N", "自定义截面(N)");
                pKeyOpts.Keywords.Add("无缝钢管(RP)", "RP", "无缝钢管(RP)");
                pKeyOpts.Keywords.Add("焊接钢管(WP)", "WP", "焊接钢管(WP)");
                pKeyRes = ed.GetKeywords(pKeyOpts);
                switch (pKeyRes.StringResult) {
                    case "自定义截面(N)":
                        var npSectionData = new List<double>();
                        pDoubleOpt = new PromptDoubleOptions("\n请输入圆管外径");
                        pDoubleRes = ed.GetDouble(pDoubleOpt);
                        if (pDoubleRes.Status != PromptStatus.OK) {
                            return;
                        }

                        npSectionData.Add(pDoubleRes.Value);
                        pDoubleOpt = new PromptDoubleOptions("\n请输入圆管壁厚");
                        pDoubleRes = ed.GetDouble(pDoubleOpt);
                        if (pDoubleRes.Status != PromptStatus.OK) {
                            return;
                        }

                        npSectionData.Add(pDoubleRes.Value);
                        CadFunctions.DrawPile(trans, ed.GetPoint(pPointOpt).Value, npSectionData[0], npSectionData[1]);
                        break;
                    case "无缝钢管(RP)":
                        pKeyOpts = new PromptKeywordOptions("\n请选择圆管截面");
                        sectionPropDic = GeneralTemplateData.RPileSectionPropDic;
                        foreach (var item in sectionPropDic) {
                            pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key);
                        }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) {
                            return;
                        }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        CadFunctions.DrawPile(trans, ed.GetPoint(pPointOpt).Value, Convert.ToDouble(sectionProp["D"]), Convert.ToDouble(sectionProp["t"]));
                        break;
                    case "焊接钢管(WP)":
                        pKeyOpts = new PromptKeywordOptions("\n请选择圆管截面");
                        sectionPropDic = GeneralTemplateData.WPileSectionPropDic;
                        foreach (var item in sectionPropDic) {
                            pKeyOpts.Keywords.Add(item.Key, item.Key, item.Key);
                        }

                        pKeyRes = ed.GetKeywords(pKeyOpts);
                        if (pKeyRes.Status != PromptStatus.OK) {
                            return;
                        }

                        sectionProp = sectionPropDic[pKeyRes.StringResult];
                        CadFunctions.DrawPile(trans, ed.GetPoint(pPointOpt).Value, Convert.ToDouble(sectionProp["D"]), Convert.ToDouble(sectionProp["t"]));
                        break;
                }

                break;
            case "矩形管(ST)":
                var stSectionData = new List<double>();
                pDoubleOpt = new PromptDoubleOptions("\n请输入矩形管宽度");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) {
                    return;
                }

                stSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入矩形管高度");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) {
                    return;
                }

                stSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入矩形管厚度");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) {
                    return;
                }

                stSectionData.Add(pDoubleRes.Value);
                pDoubleOpt = new PromptDoubleOptions("\n请输入矩形管内R角");
                pDoubleRes = ed.GetDouble(pDoubleOpt);
                if (pDoubleRes.Status == PromptStatus.Cancel) {
                    return;
                }

                stSectionData.Add(pDoubleRes.Value);
                CadFunctions.DrawSquareTube(trans, ed.GetPoint(pPointOpt).Value, stSectionData[0], stSectionData[1], stSectionData[2], stSectionData[3]);
                break;
        }
    }

    #endregion
}