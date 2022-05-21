namespace DotNetScaServerData.Models;

public class NuGetPackage
{
    public int Id { get; set; }
    
    public string PackageId { get; set; }
    public string PackageVersion { get; set; }

    public List<Signature> Signatures { get; set; } = new();
    
    public NuGetPackageToLoad LoadRequest { get; set; }
}