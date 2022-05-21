namespace DotNetScaServerData.Models;

public class NuGetPackageToLoad
{
    public int Id { get; set; }
    
    public string PackageId { get; set; }
    public string PackageVersion { get; set; }
    
    public bool IsLoading { get; set; }
    public bool IsLoaded { get; set; }
    public DateTimeOffset? LoadTime { get; set; }
    public DateTimeOffset AddTime { get; set; }
}