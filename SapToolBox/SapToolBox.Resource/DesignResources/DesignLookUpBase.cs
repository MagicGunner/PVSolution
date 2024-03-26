using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace SapToolBox.Resource.DesignResources;

public class DesignLookUpBase {
    public XDocument LoadEmbeddedXml(string resourceName) {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        return XDocument.Load(reader);
    }
}