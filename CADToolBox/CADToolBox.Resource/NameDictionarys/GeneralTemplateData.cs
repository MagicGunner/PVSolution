using System.Collections.Generic;
using System.Xml.Linq;
using System;
using System.Linq;

namespace CADToolBox.Resource.NameDictionarys;

public class SectionInfo {
    public string Name;

    public Dictionary<string, string> Props;
}

public static class GeneralTemplateData {
    public static Dictionary<string, IEnumerable<SectionInfo>> PostSectionMap { get; } = new();

    public static Dictionary<string, IEnumerable<SectionInfo>> BeamSectionMap { get; } = new();


    static GeneralTemplateData() {
        InitPostSectionMap();
        InitBeamSectionMap();
    }

    static void InitPostSectionMap() {
        // Load XML data
        var xmlDoc = XDocument.Load(@"E:\00-Code\PVSolution\CADToolBox\CADToolBox.Resource\Template\SectionData.xml");


        var root             = xmlDoc.Root;
        var templatePostList = xmlDoc.Descendants("PostSection").Elements("SectionType").Select(item => item);

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

    static void InitBeamSectionMap() {
        // Load XML data
        var xmlDoc = XDocument.Load(@"E:\00-Code\PVSolution\CADToolBox\CADToolBox.Resource\Template\SectionData.xml");


        var root             = xmlDoc.Root;
        var templateBeamList = xmlDoc.Descendants("BeamSection").Elements("SectionType").Select(item => item);

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
}