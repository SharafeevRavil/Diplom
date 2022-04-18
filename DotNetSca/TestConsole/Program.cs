// See https://aka.ms/new-console-template for more information

using DotNetSCA;
using NuGetLoader;

Console.WriteLine("TestConsole / Hello, World!");

const string solutionPath = @"C:\Users\Admin\RiderProjects\Diplom\TestSolutions\Test1\Test1.sln";
var syntaxTrees = await Test.GetSyntaxTreesFromSolution(solutionPath);
var features = await Test.GetFeatures(syntaxTrees);

//const string dllPath = @"C:\Users\Admin\RiderProjects\Diplom\TestSolutions\Newtonsoft.Json.dll";
//await Decompiler.Decompile(dllPath);
Console.WriteLine("================================================================");
foreach (var feature in features)
{
    Console.WriteLine();
    Console.WriteLine(feature);
    Console.WriteLine("----------------------------------------------------------------");
    Console.WriteLine(SimHashPython.GetSymHash(feature));
    Console.WriteLine("================================================================");
}
