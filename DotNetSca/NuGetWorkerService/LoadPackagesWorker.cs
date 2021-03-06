using DotNetSca;
using DotNetSca.SimHash;
using DotNetScaServerData.DbContext;
using DotNetScaServerData.Models;
using NuGetLoader;

namespace NuGetWorkerService;

public class LoadPackagesWorker : BackgroundService
{
    private readonly ILogger<LoadPackagesWorker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;

    public LoadPackagesWorker(ILogger<LoadPackagesWorker> logger, IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var updateTimeInSeconds = _configuration.GetValue<int>("LoadUpdateTimeInSeconds");
        var updateTimeInMs = updateTimeInSeconds * 1000;


        using var scope = _scopeFactory.CreateScope();
        var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        if (applicationDbContext == null) throw new ArgumentException();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

                var packages = applicationDbContext.NuGetPackageToLoads
                    .Where(x => !x.IsLoaded && !x.IsLoading)
                    .OrderBy(x => x.Id)
                    .ToList();
                _logger.LogInformation("Found {Count} packages to load", packages.Count);

                const int packagesLimit = 1;
                packages = packages.Take(packagesLimit).ToList();
                foreach (var package in packages)
                    package.IsLoading = true;
                await applicationDbContext.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Taking {Count} packages to load", packages.Count);

                var createdPackages = await Task.WhenAll(packages.Select(package =>
                    HandleLoadedPackage(package)));
                foreach (var createdPackage in createdPackages)
                {
                    if (createdPackage == null) continue;
                    applicationDbContext.NuGetPackages.Add(createdPackage);
                    createdPackage.LoadRequest.IsLoaded = true;
                }

                await applicationDbContext.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("Handled {Count} packages", packages.Count);
            }
            catch
            {
                // ignored
            }

            await Task.Delay(updateTimeInMs, stoppingToken);
        }
    }

    private async Task<NuGetPackage> HandleLoadedPackage(NuGetPackageToLoad package)
    {
        //????????????
        var leaf = new NuGetLoader.NuGetLoader.CatalogLeaf()
        {
            Id = package.PackageId,
            Version = package.PackageVersion
        };
        var (path, dllFiles) = await NuGetLoader.NuGetLoader.LoadNuGetPackage(leaf);
        if (path == null || dllFiles == null) return null;

        var createdPackage = new NuGetPackage()
        {
            LoadRequest = package,
            PackageId = package.PackageId,
            PackageVersion = package.PackageVersion
        };

        _logger.LogInformation("Loaded {Count} dlls for package {PackageId}@{PackageVersion}",
            dllFiles.Count, package.PackageId, package.PackageVersion);

        //????????????????????????
        var syntaxTreesArray = await Task.WhenAll(dllFiles
            .Select(dllPath => Path.Combine(path, dllPath))
            .Select(async dllPath =>
            {
                _logger.LogInformation("Decompiling {DllPath} for package {PackageId}@{PackageVersion}",
                    dllPath, package.PackageId, package.PackageVersion);
                var csPath = await Decompiler.Decompile(dllPath);
                _logger.LogInformation("Decompiled {CsPath} for package {PackageId}@{PackageVersion}",
                    csPath, package.PackageId, package.PackageVersion);
                return csPath != null
                    ? await Ast.GetSyntaxTreesFromFile(csPath)
                    : null;
            }));

        /*var syntaxTreesArray =
            await Task.WhenAll(
                dllFiles.Select(x => x)
                .Select(Ast.GetSyntaxTreesFromFile));*/
        _logger.LogInformation("Start hashing for package {PackageId}@{PackageVersion}",
            package.PackageId, package.PackageVersion);
        foreach (var syntaxTrees in syntaxTreesArray)
        {
            if (syntaxTrees == null)
                continue;

            const int limit = 10;
            var syntaxTrees2 = syntaxTrees.Take(limit);

            var featuresLists = (await Ast.GetFeaturesList(syntaxTrees2)).FilterBySize(512).ToList();
            foreach (var featuresList in featuresLists)
            {
                createdPackage.Signatures.Add(new Signature()
                {
                    Description = featuresList.Description,
                    Hash = SimHash.HashBytesToString(
                        SimHash.GenerateSimHash(featuresList.Features, SimHash.GetMd5HashFunc()))
                });
            }
            _logger.LogInformation("Syntax tree done with featuresLists {Lists} for package {PackageId}@{PackageVersion}",
                featuresLists.Count, package.PackageId, package.PackageVersion);
        }

        _logger.LogInformation("Loaded {Count} signatures for package {PackageId}@{PackageVersion}",
            createdPackage.Signatures.Count, package.PackageId, package.PackageVersion);
        NuGetLoader.NuGetLoader.ClearLoadedPackage(path);
        return createdPackage;
    }
}