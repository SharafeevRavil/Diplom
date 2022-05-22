namespace ScaWebAngular.Models;

public class Package
{
    public int Id { get; set; }
    
    public string PackageId { get; set; }
    public string PackageVersion { get; set; }
    
    public string Description { get; set; }
    public List<Vulnerability> Vulnerabilities { get; set; }
}