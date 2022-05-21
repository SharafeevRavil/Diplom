using System.Net.Http.Json;
using System.Text.Json.Serialization;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetLoader;

public class NuGetLoader
{
    private const string LoadingDirectory = @"C:\Users\Admin\Desktop\hui\";

    private static readonly SourceRepository Repository =
        NuGet.Protocol.Core.Types.Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

    public static async Task<List<CatalogLeaf>> GetNewPackages(DateTime dateFrom)
    {
        //var packagesPublishedAfter = DateTime.UtcNow.AddHours(-1);

        /*var logger = NullLogger.Instance;
        var cancellationToken = CancellationToken.None;

        var cache = new SourceCacheContext();*/
        //_repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        /*var resource = await repository.GetResourceAsync<FindPackageByIdResource>(cancellationToken);

        
        var searchResource = await repository.GetResourceAsync<PackageSearchResource>(cancellationToken);
        var res = await searchResource.SearchAsync(
            "Newtonsoft.Json",
            new SearchFilter(false, SearchFilterType.IsLatestVersion),
            0, 100, logger, cancellationToken);*/

        var serviceIndex = await Repository.GetResourceAsync<ServiceIndexResourceV3>();
        var catalogIndexUrl = serviceIndex.GetServiceEntryUri("Catalog/3.0.0");

        using var httpClient = new HttpClient();
        var index = await httpClient.GetFromJsonAsync<CatalogIndex>(catalogIndexUrl);
        var pageItems = index.Items.Where(x => x.CommitTimeStamp > dateFrom);

        var leaves = new List<CatalogLeaf>();
        foreach (var pageItem in pageItems)
        {
            var page = await httpClient.GetFromJsonAsync<CatalogPage>(pageItem.Url);
            leaves.AddRange(page.Items.Where(x => x.CommitTimeStamp > dateFrom));
        }

        /*foreach (var leaf in leaves.OrderBy(x => x.CommitTimeStamp))
        {
            Console.WriteLine($"{leaf.Id}@{leaf.Version} - {leaf.Url}");

            // Get the full data about the leaf item
            //var str = await httpClient.GetStringAsync(leaf.Url);
        }*/

        /*IEnumerable<NuGetVersion> versions = await resource.GetAllVersionsAsync(
            "Newtonsoft.Json",
            cache,
            logger,
            cancellationToken);

        foreach (NuGetVersion version in versions)
        {
            Console.WriteLine($"Found version {version}");
        }*/

        return leaves.OrderBy(x => x.CommitTimeStamp).ToList();
        ///////////await LoadNuGetPackages(repository, leaves);
    }

    public static async Task<List<(string path, List<string> dllFiles)>> LoadNuGetPackages(
        IEnumerable<CatalogLeaf> leaves) =>
        (await Task.WhenAll(leaves.Select(package => LoadNuGetPackage(package))))
        .Where(x => x.dllFiles.Count > 0)
        .ToList();

    public static async Task<(string path, List<string> dllFiles)> LoadNuGetPackage(CatalogLeaf package)
    {
        var findPackageByIdResource = await Repository.GetResourceAsync<FindPackageByIdResource>();

        var logger = NullLogger.Instance;
        var cancellationToken = CancellationToken.None;
        var cache = new SourceCacheContext();

        await using (var packageStream = new MemoryStream())
        {
            ////Console.WriteLine($"First package: {package.Id}@{package.Version}");

            await findPackageByIdResource.CopyNupkgToStreamAsync(
                package.Id,
                new NuGetVersion(package.Version),
                packageStream,
                cache,
                logger,
                cancellationToken);

            ////Console.WriteLine($"Stream length: {packageStream.Length}");

            if (packageStream.Length == 0)
                return await Task.FromResult<(string, List<string>)>((null, null));

            using (var packageReader = new PackageArchiveReader(packageStream))
            {
                var nuspecReader = await packageReader.GetNuspecReaderAsync(cancellationToken);

                ////Console.WriteLine($"Tags: {nuspecReader.GetTags()}");
                ////Console.WriteLine($"Description: {nuspecReader.GetDescription()}");

                /*Console.WriteLine("Files:");
                //var packageSaveMode = PackageSaveMode.Files;
                foreach (var file in await packageReader.GetFilesAsync(cancellationToken))
                {
                    Console.WriteLine($" - {file} - {PackageHelper.IsAssembly(file)}");
                }*/

                //packageReader.CopyFiles("", packageReader.GetFiles(), (file, path, stream) => )
                //
                var packageFiles = (await packageReader.GetPackageFilesAsync(PackageSaveMode.Files, cancellationToken))
                    .ToList();
                var packageFileExtractor = new PackageFileExtractor(packageFiles, XmlDocFileSaveMode.Skip);
                
                var path1 = $"./{package.Id}@{package.Version}_{Guid.NewGuid()}";
                var path = $@"{LoadingDirectory}{path1}";
                await packageReader.CopyFilesAsync(
                    path,
                    packageFiles,
                    packageFileExtractor.ExtractPackageFile,
                    logger, cancellationToken);

                var dllFiles = packageFiles
                    .Where(PackageHelper.IsAssembly)
                    .Where(x => !x.StartsWith("tools/"))
                    .ToList();
                ////foreach (var dllFile in dllFiles)
                ////{
                    ////Console.WriteLine($"Dll file: {dllFile}");
                    ////}

                return (path, dllFiles);
            }
        }
    }

    public static void ClearLoadedPackage(string path)
    {
        Directory.Delete(path, true);
    }

    public abstract class CatalogEntity
    {
        [JsonPropertyName("@id")] public string Url { get; set; }

        [JsonPropertyName("commitTimeStamp")] public DateTime CommitTimeStamp { get; set; }
    }

    public sealed class CatalogIndex : CatalogEntity
    {
        [JsonPropertyName("items")] public List<CatalogPage> Items { get; set; }
    }

    public sealed class CatalogPage : CatalogEntity
    {
        [JsonPropertyName("items")] public List<CatalogLeaf> Items { get; set; }
    }

    public sealed class CatalogLeaf : CatalogEntity
    {
        [JsonPropertyName("nuget:id")] public string Id { get; set; }

        [JsonPropertyName("nuget:version")] public string Version { get; set; }

        [JsonPropertyName("@type")] public string Type { get; set; }
    }
}