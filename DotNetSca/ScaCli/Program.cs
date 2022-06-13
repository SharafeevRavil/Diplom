using System.CommandLine;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DotNetSca;
using DotNetSca.SimHash;
using NuGetLoader;
using Shared.Dto.Packages;
using Shared.Dto.Scan;
using Shared.Dto.Signatures;

namespace ScaCli;

public static class Program
{
    public static async Task Main(string scaWebEndpoint, string projectId, string jwtToken, string dirPath = ".")
    {
        if (string.IsNullOrWhiteSpace(scaWebEndpoint))
        {
            Console.WriteLine("Please provide scaWebEndpoint (--sca-web-endpoint)");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(projectId))
        {
            Console.WriteLine("Please provide projectId (--project-id)");
            return;
        }

        if (string.IsNullOrWhiteSpace(jwtToken))
        {
            Console.WriteLine("Please provide jwt token (--jwt-token)");
            return;
        }

        if (string.IsNullOrWhiteSpace(dirPath))
        {
            Console.WriteLine("Please provide directory path (--dir-path)");
            return;
        }

        await DoWork(scaWebEndpoint, projectId, jwtToken, dirPath);
    }

    public static async Task DoWork(string scaWebEndpoint, string projectId, string jwtToken, string dirPath)
    {
        Console.WriteLine($"scaWebEndpoint = {scaWebEndpoint}");
        Console.WriteLine($"jwtToken = {jwtToken}");
        Console.WriteLine($"dirPath = {dirPath}");
        
        //Validation
        var solutionPath = Directory.GetFiles(dirPath, "*.sln").FirstOrDefault();
        if (solutionPath == null)
        {
            Console.WriteLine("Solution not found in directory");
            return;
        }

        var scan = new ScanDto();
        scan.ProjectId = int.Parse(projectId);
        if (scan.ProjectId < 1)
        {
            Console.WriteLine("Wrong project id");
            return;
        }
            
        //Direct packages
        var foundDirectPackages = DirectDependenciesDetector.GetDependencies(dirPath)
            .ToList();
        Console.WriteLine($"Found direct packages: {foundDirectPackages.Count}");
        foreach (var package in foundDirectPackages)
        {
            Console.WriteLine(package.Include + " " + package.Version);
        }

        scan.ExplicitPackages = foundDirectPackages
            .Select(x => new PackageDto()
            {
                PackageId = x.Include,
                PackageVersion = x.Version.ToString()
            })
            .ToList();
        
        //Signatures
        var syntaxTrees = await Ast.GetSyntaxTreesFromSolution(solutionPath);
        var featuresList = await Ast.GetFeaturesList(syntaxTrees);

        var signatures = featuresList
            .FilterBySize(512)
            .Select(f => new SignatureDescriptionDto
            {
                Description = f.Description,
                Hash = SimHash.HashBytesToString(SimHash.GenerateSimHash(f.Features, SimHash.GetMd5HashFunc())),
            })
            .ToList();
        Console.WriteLine($"Calculated signatures: {signatures.Count}");
        scan.Signatures = signatures;
        
        //Send to server
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        Console.WriteLine($"Sending scan data");
        var resp = await httpClient.PostAsJsonAsync($"{scaWebEndpoint}/api/reports/submit", scan);
        Console.WriteLine($"Server response: {resp.StatusCode} | {await resp.Content.ReadAsStringAsync()}");
    }
}