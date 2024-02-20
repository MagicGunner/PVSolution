using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SapToolBox.Base.DesignResources;

public class DesignLookup {
    private static DesignLookup _instance;

    public static DesignLookup Instance => _instance ??= new DesignLookup();

    // 中国冷弯规范中轴心受压稳定性系数
    public Dictionary<int, double> ChineseColdFormedPhiDic { get; }

    // 中国冷弯规范中卷边的最小宽厚比
    public Dictionary<int, double> ChineseColdFormedLipMinAtRatio { get; } =
        new() {
                  { 15, 5.4 },
                  { 20, 6.3 },
                  { 25, 7.2 },
                  { 30, 8.0 },
                  { 35, 8.5 },
                  { 40, 9.0 },
                  { 45, 9.5 },
                  { 50, 10.0 },
                  { 55, 10.5 },
                  { 60, 11.0 }
              };

    private DesignLookup() {
        // 加载 XML 文件
        XDocument xmlDoc;
        try {
            xmlDoc
                = XDocument.Load(@"E:\BaiduSyncdisk\Works\00-Research_and_development\PVSolution\SapToolBox\SapToolBox.Base\DesignResources\DesignResources.xml");
        } catch {
            xmlDoc
                = XDocument.Load(@"E:\work\Code\PVSolution\SapToolBox\SapToolBox.Base\DesignResources\DesignResources.xml");
        }


        var result = xmlDoc.Descendants("Resource")
                           .ToDictionary(resource => resource.Attribute("Name")?.Value,
                                         resource => resource.Elements("Item")
                                                             .ToDictionary(item => int.Parse(item.Attribute("lambda")
                                                                             ?.Value
                                                                        ?? "0"),
                                                                           item =>
                                                                               double.Parse(item.Attribute("Phi")
                                                                                     ?.Value
                                                                                ?? "0.0")));
        ChineseColdFormedPhiDic = result["ColdFormedPhi"];
    }

    // 后面考虑使用泛型函数
    /// <summary>
    /// 获取部分加筋板件翻边的最小宽厚比
    /// </summary>
    /// <param name="btRadio">加劲板件的宽厚比</param>
    /// <returns></returns>
    public double GetChineseColdFormedLipMinAtRatio(double btRadio) {
        switch (btRadio) {
            case < 15:  return 0;
            case >= 60: return ChineseColdFormedLipMinAtRatio[60];
            default: {
                var lowerIndex = (int)Math.Floor(btRadio / 5)       * 5;
                var upperIndex = ((int)Math.Floor(btRadio / 5) + 1) * 5;
                var lower      = ChineseColdFormedLipMinAtRatio[lowerIndex];
                var upper      = ChineseColdFormedLipMinAtRatio[upperIndex];
                return lower + (upper - lower) / 5 * (btRadio - lowerIndex);
                break;
            }
        }
    }


    public double GetChineseColdFormedPhi(double lambda) {
        switch (lambda) {
            case 0:      return 1;
            case >= 250: return ChineseColdFormedPhiDic[250];
            default: {
                var lowerIndex = (int)Math.Floor(lambda);
                var upperIndex = (int)Math.Floor(lambda) + 1;
                var lower      = ChineseColdFormedPhiDic[lowerIndex];
                var upper      = ChineseColdFormedPhiDic[upperIndex];
                return lower + (upper - lower) / 1 * (lambda - lowerIndex);
                break;
            }
        }
    }
}