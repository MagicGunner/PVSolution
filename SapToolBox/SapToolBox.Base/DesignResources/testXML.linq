<Query Kind="Statements">
  <NuGetReference>CommunityToolkit.Mvvm</NuGetReference>
  <NuGetReference>DocumentFormat.OpenXml</NuGetReference>
  <Namespace>CommunityToolkit.Mvvm.Messaging</Namespace>
  <Namespace>CommunityToolkit.Mvvm.Messaging.Messages</Namespace>
</Query>

// 加载 XML 文件
XDocument xmlDoc = XDocument.Load(@"E:\BaiduSyncdisk\Works\00-Research_and_development\PVSolution\SapToolBox\SapToolBox.Base\DesignResources\DesignResources.xml");

XElement root = xmlDoc.Root;

 Dictionary<string, Dictionary<int, double>> result = xmlDoc.Descendants("Resource")
            .ToDictionary(
                resource => resource.Attribute("Name")?.Value,
                resource => resource.Elements("Item")
                    .ToDictionary(
                        item => int.Parse(item.Attribute("lambda")?.Value ?? "0"),
                        item => double.Parse(item.Attribute("Phi")?.Value ?? "0.0")
                    )
            );

result.Dump();