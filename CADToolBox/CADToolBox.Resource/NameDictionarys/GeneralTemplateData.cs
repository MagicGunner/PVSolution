using System.Collections.Generic;
using System.Xml.Linq;
using System;
using System.Linq;
using System.IO;
using System.Reflection;

namespace CADToolBox.Resource.NameDictionarys;

public class SectionInfo {
    public string Name;

    public Dictionary<string, string> Props;
}

public static class GeneralTemplateData {
    public static Dictionary<string, IEnumerable<SectionInfo>> PostSectionMap { get; } = new();

    public static Dictionary<string, IEnumerable<SectionInfo>> BeamSectionMap { get; } = new();

    private static readonly XDocument SectionXDocument
        = LoadEmbeddedXml("CADToolBox.Resource.Template.SectionData.xml");


    static GeneralTemplateData() {
        InitPostSectionMap();
        InitBeamSectionMap();
    }

    private static void InitPostSectionMap() {
        var templatePostList = SectionXDocument.Descendants("PostSection").Elements("SectionType").Select(item => item);

        foreach (var element in templatePostList) {
            var sectionList = element.Elements("Section").Select(item => new SectionInfo {
                                                                             Name = item.Attribute("Name")!.Value,
                                                                             Props = item.Attributes()
                                                                                .Where(i => i.Name.LocalName !=
                                                                                     "Name")
                                                                                .ToDictionary(i => i.Name.LocalName,
                                                                                     i => i.Value)
                                                                         });
            PostSectionMap.Add(element.Attribute("Name")!.Value, sectionList);
        }
    }

    private static void InitBeamSectionMap() {
        var templateBeamList = SectionXDocument.Descendants("BeamSection").Elements("SectionType").Select(item => item);

        foreach (var element in templateBeamList) {
            var sectionList = element.Elements("Section").Select(item => new SectionInfo {
                                                                             Name = item.Attribute("Name")!.Value,
                                                                             Props = item.Attributes()
                                                                                .Where(i => i.Name.LocalName !=
                                                                                     "Name")
                                                                                .ToDictionary(i => i.Name.LocalName,
                                                                                     i => i.Value)
                                                                         });
            BeamSectionMap.Add(element.Attribute("Name")!.Value, sectionList);
        }
    }

    private static XDocument LoadEmbeddedXml(string resourceName) {
        var       assembly = Assembly.GetExecutingAssembly();
        using var stream   = assembly.GetManifestResourceStream(resourceName);
        using var reader   = new StreamReader(stream!);
        return XDocument.Load(reader);
    }
}