using DotNetScaServerData.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetScaServerData.DbContext;

public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<NuGetPackageToLoad> NuGetPackageToLoads { get; set; }
    public DbSet<NuGetLoads> NuGetLoads { get; set; }
    public DbSet<NuGetPackage> NuGetPackages { get; set; }
    public DbSet<Signature> Signatures { get; set; }
    
    public DbSet<UserAuthentication> UserAuthentications { get; set; }
}