using System.Xml.Linq;
using System.Xml.XPath;

namespace NuGetLoader;

public static class DirectDependenciesDetector
{
    public static IEnumerable<PackageReference> GetDependencies(string dirPath)
    {
        var files = Directory.GetFiles(dirPath, "*.csproj", SearchOption.AllDirectories);
        return files
            .SelectMany(x => GetDependenciesInXml(File.ReadAllText(x)))
            .DistinctBy(x => x.Include+x.Version);
    }

    private static IEnumerable<PackageReference> GetDependenciesInXml(string xml)
    {
        var doc = XDocument.Parse(xml);
        var packageReferences = doc.XPathSelectElements("//PackageReference")
            .Select(pr =>
            {
                var version = pr.Attribute("Version").Value;
                var dashIndex = version.IndexOf("-");
                if(dashIndex > -1)
                    version = version.Substring(0, dashIndex);

                return new PackageReference
                {
                    Include = pr.Attribute("Include").Value,
                    Version = new Version(version)
                };
            });

        return packageReferences;
    }

    public class PackageReference
    {
        public string Include { get; set; }
        public Version Version { get; set; }
    }
}