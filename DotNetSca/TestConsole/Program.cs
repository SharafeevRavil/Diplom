// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Running;
using DotNetSca;
using DotNetSCA;
using DotNetSca.SimHash;
using NuGetLoader;

//using SimhashLib;

//using Simhash.NetCore;

Console.WriteLine("TestConsole / Hello, World!");

BenchmarkRunner.Run<BenchmarkTest>();
return 0;

const string solutionPath = @"C:\Users\Admin\RiderProjects\Diplom\TestSolutions\Test1\Test1.sln";
var syntaxTrees = await Test.GetSyntaxTreesFromSolution(solutionPath);
var features = await Test.GetFeatures(syntaxTrees);

//const string dllPath = @"C:\Users\Admin\RiderProjects\Diplom\TestSolutions\Newtonsoft.Json.dll";
//await Decompiler.Decompile(dllPath);
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
foreach (var feature in features)
{
    Console.WriteLine();
    Console.WriteLine(feature);
    Console.WriteLine("----------------------------------------------------------------");
    Console.WriteLine(SimHashPython.GetSymHash(feature));
    Console.WriteLine(SimHash.HashBytesToString(SimHash.GenerateSimHash(NGrams(feature, 9), SimHash.GetMd5HashFunc())));
    Console.WriteLine("================================================================");
}

Console.WriteLine(Hamming.Distance(
    SimHash.GenerateSimHash(NGrams(features.ToList()[features.Count() - 2], 9), SimHash.GetMd5HashFunc()),
    SimHash.GenerateSimHash(NGrams(features.ToList()[features.Count() - 1], 9), SimHash.GetMd5HashFunc())));
