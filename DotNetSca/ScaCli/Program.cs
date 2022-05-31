using System.CommandLine;
using DotNetSca;
using DotNetSca.SimHash;
using NuGetLoader;

namespace ScaCli;

public static class Program
{
    public static async Task Main(string scaWebEndpoint, string jwtToken, string dirPath = ".")
    {
        if (string.IsNullOrWhiteSpace(scaWebEndpoint))
        {
            Console.WriteLine("Please provide scaWebEndpoint (--sca-web-endpoint)");
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
        
        await DoWork(scaWebEndpoint, jwtToken, dirPath);
    }

    public static async Task DoWork(string scaWebEndpoint, string jwtToken, string dirPath)
    {
        Console.WriteLine($"scaWebEndpoint = {scaWebEndpoint}");
        Console.WriteLine($"jwtToken = {jwtToken}");
        Console.WriteLine($"dirPath = {dirPath}");

        var foundDirectPackages = DirectDependenciesDetector.GetDependencies(dirPath)
            .ToList();
        Console.WriteLine($"Found direct packages: {foundDirectPackages.Count}");
        foreach (var package in foundDirectPackages)
        {
            Console.WriteLine(package.Include + " " + package.Version);
        }


        var solutionPath = Directory.GetFiles(dirPath, "*.sln").FirstOrDefault();
        if (solutionPath == null)
        {
            Console.WriteLine("Solution not found in directory");
            return;
        }

        var syntaxTrees = await Ast.GetSyntaxTreesFromSolution(solutionPath);
        var featuresList = await Ast.GetFeaturesList(syntaxTrees);

        var hashes = featuresList
            .FilterBySize(512)
            .Select(f => new
            {
                Description = f.Description,
                Hash = SimHash.HashBytesToString(SimHash.GenerateSimHash(f.Features, SimHash.GetMd5HashFunc()))
            })
            .ToList();
        Console.WriteLine($"Calculated hashes: {hashes.Count}");
    }
}