namespace ScaWebAngular.Models;

public class Signature
{
    public int Id { get; set; }
    
    public string Hash { get; set; }
    public string Description { get; set; }
    public List<Package> FoundPackages { get; set; }
}