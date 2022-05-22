using Microsoft.AspNetCore.Identity;

namespace ScaWebAngular.Models;

public class ApplicationUser : IdentityUser
{
    public List<Project> AllowedProjects { get; set; }
}