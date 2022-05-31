// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using DotNetSca;
using DotNetSca.SimHash;
using NuGetLoader;

public static class Program
{
    static async Task RunSimHashes()
    {
        /*const string solutionPath = @"C:\Users\Admin\RiderProjects\Diplom\TestSolutions\Test1\Test1.sln";
        var syntaxTrees = await Test.GetSyntaxTreesFromSolution(solutionPath);*/
        
        const string dllPath = @"C:\Users\Admin\RiderProjects\Diplom\TestSolutions\Newtonsoft.Json.dll.cs";
        var syntaxTrees = await Ast.GetSyntaxTreesFromFile(dllPath);
        
        var featuresList = await Ast.GetFeaturesList(syntaxTrees);
        
        static IEnumerable<string> NGrams(string input, int n)
        {
            var list = new List<string>();
            for (var i = 0; i <= input.Length - n; i++)
            {
                list.Add(input.Substring(i, n));
            }

            return list;
        }

        Console.WriteLine("================================================================");
        foreach (var features in featuresList)
        {
            Console.WriteLine();
            foreach (var f in features.Features.Select((x, i) => $"{i}: {x}"))
            {
                Console.WriteLine(f);
            }

            Console.WriteLine("----------------------------------------------------------------");
            //Console.WriteLine(SimHashPython.GetSymHash(feature));
            Console.WriteLine(
                SimHash.HashBytesToString(SimHash.GenerateSimHash(features.Features, SimHash.GetMd5HashFunc())));
            Console.WriteLine("================================================================");
        }

        /*Console.WriteLine(Hamming.Distance(
            SimHash.GenerateSimHash(NGrams(features.ToList()[features.Count() - 2], 9), SimHash.GetMd5HashFunc()),
            SimHash.GenerateSimHash(NGrams(features.ToList()[features.Count() - 1], 9), SimHash.GetMd5HashFunc())));*/
    }

    static void RunBenchmarks()
    {
        BenchmarkRunner.Run<BenchmarkTest>();
    }

    static async Task Decompile()
    {
        const string dllPath = @"C:\Users\Admin\RiderProjects\Diplom\TestSolutions\Newtonsoft.Json.dll";
        await Decompiler.Decompile(dllPath);
    }

    static async Task Nuget()
    {
        var a = await NuGetLoader.NuGetLoader.GetNewPackages(DateTime.UtcNow.AddHours(-1));
        var b = await NuGetLoader.NuGetLoader.LoadNuGetPackages(a);
    }

    static async Task Direct()
    {
        const string dirPath = @"C:\Users\Admin\RiderProjects\Diplom\DotNetSca";
        var a = DirectDependenciesDetector.GetDependencies(dirPath);
    }

    public static async Task Main()
    {
        Console.WriteLine("TestConsole / Hello, World!");
        
        //await Nuget();
        
        //RunBenchmarks();
        
        //await RunSimHashes();
        
        await Direct();
    }
}