using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;
using ScaWebAngular.Models;

namespace ScaWebAngular.Data;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Signature> Signatures { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Vulnerability> Vulnerabilities { get; set; }

    public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
        : base(options, operationalStoreOptions)
    {
    }
}