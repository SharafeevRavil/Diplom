namespace DotNetScaServerData.Models;

public class Signature
{
    public int Id { get; set; }
    
    public string Description { get; set; }
    public string Hash { get; set; }
    
    public NuGetPackage Package { get; set; }
}