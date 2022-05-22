namespace ScaWebAngular.Models;

public class Project
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    
    public List<Report> Reports { get; set; }
    public List<ApplicationUser> AllowedUsers { get; set; }
}