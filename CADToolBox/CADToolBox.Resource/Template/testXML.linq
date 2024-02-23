<Query Kind="Statements">
  <NuGetReference>CommunityToolkit.Mvvm</NuGetReference>
  <NuGetReference>DocumentFormat.OpenXml</NuGetReference>
  <Namespace>CommunityToolkit.Mvvm.Messaging</Namespace>
  <Namespace>CommunityToolkit.Mvvm.Messaging.Messages</Namespace>
</Query>

// 加载 XML 文件
XDocument xmlDoc = XDocument.Load(@"E:\BaiduSyncdisk\Works\00-Research_and_development\PVSolution\CADToolBox\AutoGA\Template\SectionData.xml");

XElement root = xmlDoc.Root;

var templatePostList = xmlDoc.Descendants("PostSection").Elements("SectionType").Select(item => item);
var sectionMap = new Dictionary<string, IEnumerable<SectionInfo>>();

foreach (var element in templatePostList)
{
	var sectionList = element.Elements("Section").Select(item => new SectionInfo {
		Name = item.Attribute("Name").Value,
		Props = item.Attributes().Where(i => i.Name.LocalName != "Name").ToDictionary(i => i.Name.LocalName, i => i.Value)
	});
	
	
	sectionMap.Add(element.Attribute("Name").Value, sectionList);
}


sectionMap.Dump();

class SectionInfo {
	public string Name;
	public Dictionary<string, string> Props;
	
}