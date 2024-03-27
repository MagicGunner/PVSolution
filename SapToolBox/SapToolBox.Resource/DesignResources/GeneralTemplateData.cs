using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace SapToolBox.Resource.DesignResources;

public class SectionData {
    public string Name;

    public Dictionary<string, string> Props;
}

public static class GeneralTemplateData {
    public static Dictionary<string, IEnumerable<SectionData>> PostSectionMap { get; } = new();

    public static Dictionary<string, IEnumerable<SectionData>> BeamSectionMap { get; } = new();

    public static Dictionary<string, Dictionary<string, string>> WSectionPropDic            { get; }
    public static Dictionary<string, Dictionary<string, string>> RollHWSectionPropDic       { get; }
    public static Dictionary<string, Dictionary<string, string>> RollHMSectionPropDic       { get; }
    public static Dictionary<string, Dictionary<string, string>> RollHNSectionPropDic       { get; }
    public static Dictionary<string, Dictionary<string, string>> RollHTSectionPropDic       { get; }
    public static Dictionary<string, Dictionary<string, string>> RollCSectionPropDic        { get; }
    public static Dictionary<string, Dictionary<string, string>> RollEqualLSectionPropDic   { get; }
    public static Dictionary<string, Dictionary<string, string>> RollUnEqualLSectionPropDic { get; }
    public static Dictionary<string, Dictionary<string, string>> RPileSectionPropDic        { get; }
    public static Dictionary<string, Dictionary<string, string>> WPileSectionPropDic        { get; }

    private static readonly XDocument SectionXDocument = LoadEmbeddedXml("SapToolBox.Resource.Template.SectionData.xml");


    static GeneralTemplateData() {
        InitPostSectionMap();
        InitBeamSectionMap();

        WSectionPropDic = PostSectionMap["W型钢"].Select(item => item).ToList().ToDictionary(item => item.Name, item => item.Props);
        RollHWSectionPropDic = PostSectionMap["宽翼缘H型钢(HW)"].Select(item => item).ToList().ToDictionary(item => item.Name, item => item.Props);
        RollHMSectionPropDic = PostSectionMap["中翼缘H型钢(HM)"].Select(item => item).ToList().ToDictionary(item => item.Name, item => item.Props);
        RollHNSectionPropDic = PostSectionMap["窄翼缘H型钢(HN)"].Select(item => item).ToList().ToDictionary(item => item.Name, item => item.Props);
        RollHTSectionPropDic = PostSectionMap["薄壁H型钢(HT)"].Select(item => item).ToList().ToDictionary(item => item.Name, item => item.Props);
        RollCSectionPropDic = PostSectionMap["热轧槽钢"].Select(item => item).ToList().ToDictionary(item => item.Name, item => item.Props);
        RollEqualLSectionPropDic = PostSectionMap["热轧等边角钢"].Select(item => item).ToList().ToDictionary(item => item.Name, item => item.Props);
        RollUnEqualLSectionPropDic = PostSectionMap["热轧不等边角钢"].Select(item => item).ToList().ToDictionary(item => item.Name, item => item.Props);
        RPileSectionPropDic = PostSectionMap["无缝钢管"].Select(item => item).ToList().ToDictionary(item => item.Name, item => item.Props);
        WPileSectionPropDic = PostSectionMap["焊接钢管"].Select(item => item).ToList().ToDictionary(item => item.Name, item => item.Props);
    }

    private static void InitPostSectionMap() {
        var templatePostList = SectionXDocument.Descendants("PostSection").Elements("SectionType").Select(item => item);

        foreach (var element in templatePostList) {
            var sectionList = element.Elements("Section").Select(item => new SectionData {
                                                                                             Name = item.Attribute("Name")!.Value,
                                                                                             Props = item.Attributes().Where(i => i.Name.LocalName != "Name")
                                                                                                         .ToDictionary(i => i.Name.LocalName, i => i.Value)
                                                                                         });
            PostSectionMap.Add(element.Attribute("Name")!.Value, sectionList);
        }
    }

    private static void InitBeamSectionMap() {
        var templateBeamList = SectionXDocument.Descendants("BeamSection").Elements("SectionType").Select(item => item);

        foreach (var element in templateBeamList) {
            var sectionList = element.Elements("Section").Select(item => new SectionData {
                                                                                             Name = item.Attribute("Name")!.Value,
                                                                                             Props = item.Attributes().Where(i => i.Name.LocalName != "Name")
                                                                                                         .ToDictionary(i => i.Name.LocalName, i => i.Value)
                                                                                         });
            BeamSectionMap.Add(element.Attribute("Name")!.Value, sectionList);
        }
    }

    private static XDocument LoadEmbeddedXml(string resourceName) {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        return XDocument.Load(reader);
    }
}