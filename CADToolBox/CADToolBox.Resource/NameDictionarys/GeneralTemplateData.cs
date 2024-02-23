using System.Collections.Generic;
using System.Xml.Linq;
using System;
using System.Linq;

namespace CADToolBox.Resource.NameDictionarys;

public static class GeneralTemplateData {
    public Dictionary<string, IEnumerable<SectionInfo>> PostSectionMap { get; }

    public Dictionary<string, IEnumerable<SectionInfo>> BeamSectionMap { get; }


    static GeneralTemplateData() {
        PostSectionMap = new Dictionary<string, IEnumerable<SectionInfo>>();
        BeamSectionMap = new Dictionary<string, IEnumerable<SectionInfo>>();
        InitPostSectionMap();
        InitBeamSectionMap();
    }

    void InitPostSectionMap() {
        // Load XML data
        XDocument xmlDoc;
        try {
            xmlDoc = XDocument.Load(@"E:\work\Code\PVSolution\CADToolBox\AutoGA\Template\SectionData.xml");
        } catch (Exception) {
            xmlDoc
                = XDocument.Load(@"E:\BaiduSyncdisk\Works\00-Research_and_development\PVSolution\CADToolBox\AutoGA\Template\SectionData.xml");
        }

        XElement root             = xmlDoc.Root;
        var      templatePostList = xmlDoc.Descendants("PostSection").Elements("SectionType").Select(item => item);

        foreach (var element in templatePostList) {
            var sectionList = element.Elements("Section").Select(item => new SectionInfo {
                                                                             Name = item.Attribute("Name").Value,
                                                                             Props = item.Attributes()
                                                                                .Where(i => i.Name.LocalName !=
                                                                                     "Name")
                                                                                .ToDictionary(i => i.Name.LocalName,
                                                                                     i => i.Value)
                                                                         });
            PostSectionMap.Add(element.Attribute("Name").Value, sectionList);
        }
    }

    void InitBeamSectionMap() {
        // Load XML data
        XDocument xmlDoc;
        try {
            xmlDoc = XDocument.Load(@"E:\work\Code\PVSolution\CADToolBox\AutoGA\Template\SectionData.xml");
        } catch (Exception) {
            xmlDoc
                = XDocument.Load(@"E:\BaiduSyncdisk\Works\00-Research_and_development\PVSolution\CADToolBox\AutoGA\Template\SectionData.xml");
        }

        XElement root             = xmlDoc.Root;
        var      templateBeamList = xmlDoc.Descendants("BeamSection").Elements("SectionType").Select(item => item);

        foreach (var element in templateBeamList) {
            var sectionList = element.Elements("Section").Select(item => new SectionInfo {
                                                                             Name = item.Attribute("Name").Value,
                                                                             Props = item.Attributes()
                                                                                .Where(i => i.Name.LocalName !=
                                                                                     "Name")
                                                                                .ToDictionary(i => i.Name.LocalName,
                                                                                     i => i.Value)
                                                                         });
            BeamSectionMap.Add(element.Attribute("Name").Value, sectionList);
        }
    }
}