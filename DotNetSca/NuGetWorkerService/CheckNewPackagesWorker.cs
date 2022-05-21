using DotNetScaServerData.DbContext;
using DotNetScaServerData.Models;

namespace NuGetWorkerService;

public class CheckNewPackagesWorker : BackgroundService
{
    private readonly ILogger<CheckNewPackagesWorker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;

    public CheckNewPackagesWorker(ILogger<CheckNewPackagesWorker> logger, IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var updateTimeInMinutes = _configuration.GetValue<int>("CheckUpdateTimeInMinutes");
        var updateTimeInMs = updateTimeInMinutes * 60 * 1000;

        using var scope = _scopeFactory.CreateScope();
        var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        if (applicationDbContext == null) throw new ArgumentException();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var runningTime = DateTimeOffset.Now;
                _logger.LogInformation("Worker running at: {Time}", runningTime);

                var lastFromDb = applicationDbContext.NuGetLoads.OrderByDescending(x => x.Date).FirstOrDefault();
                var lastTime = lastFromDb?.Date.UtcDateTime ?? DateTime.UtcNow.AddHours(-1);

                var newPackages = await NuGetLoader.NuGetLoader.GetNewPackages(lastTime);
                _logger.LogInformation("Found new {Count} packages from {Time}", newPackages.Count, lastTime);

                foreach (var package in newPackages)
                {
                    if (applicationDbContext.NuGetPackageToLoads.All(x =>
                            x.PackageId != package.Id || x.PackageVersion != package.Version))
                    {
                        applicationDbContext.NuGetPackageToLoads.Add(new NuGetPackageToLoad()
                        {
                            PackageId = package.Id,
                            PackageVersion = package.Version,
                            AddTime = runningTime.ToUniversalTime(),
                            IsLoaded = false,
                            IsLoading = false,
                            LoadTime = null,
                        });
                    }
                }

                applicationDbContext.NuGetLoads.Add(new NuGetLoads()
                {
                    Date = runningTime.ToUniversalTime(),
                    LoadCount = newPackages.Count
                });
                await applicationDbContext.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Saved {Count} packages from {Time}", newPackages.Count, lastTime);
            }
            catch
            {
                // ignored
            }

            await Task.Delay(updateTimeInMs, stoppingToken);
        }
    }
}