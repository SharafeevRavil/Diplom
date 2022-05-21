using DotNetScaServerData.DbContext;
using Microsoft.EntityFrameworkCore;
using NuGetWorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
        services.AddHostedService<CheckNewPackagesWorker>();
        services.AddHostedService<LoadPackagesWorker>();
    })
    .Build();

await host.RunAsync();