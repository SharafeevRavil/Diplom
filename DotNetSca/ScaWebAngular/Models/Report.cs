namespace ScaWebAngular.Models;

public class Report
{
    public int Id { get; set; }
    
    public ApplicationUser Initiator { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public bool IsFilledByServer { get; set; }
    
    public List<Signature> GeneratedSignatures { get; set; }
    public List<Package> ExplicitPackages { get; set; }
}