using BenchmarkDotNet.Attributes;
using DotNetSca;
using DotNetSCA;
using DotNetSca.SimHash;

public class BenchmarkTest
{
    static List<string> NGrams(string input, int n){
        var list = new List<string>();
        for(var i = 0; i <= input.Length - n; i++){
            list.Add(input.Substring(i, n));
        }
        return list;
    }

    private List<string> data;
    private string data0;
    
    [GlobalSetup]
    public void Setup()
    {
        data0 = @"Console.WriteLine(""Hello, Worldoo!"");
Console.WriteLine(""Hello, Worldoo!"");
Console.WriteLine(""Hello, Worldoo!"");
Console.WriteLine(""Hello, Worldoo!"");
Console.WriteLine(""Hello, Worldoo!"");
Console.WriteLine(""Hello, Worldoo!"");
Console.WriteLine(""Hello, Worldoo!"");
Console.WriteLine(""Hello, Worldoo!"");
Console.WriteLine(""Hello, Worldoo!"");
Console.WriteLine(""Hello, Worldoo!"");
Console.WriteLine(""Hello, Worldoo!"");
Console.WriteLine(""Hello, Worldoo!"");
Console.WriteLine(""Hello, Worldoo!"");";
        data = NGrams(data0, 9);
    }

    [Benchmark]
    public void Test1()
    {
        SimHashPython.GetSymHash(data0);
    }

    [Benchmark]
    public void Test2()
    {
        SimHash.GenerateSimHash(data, SimHash.GetMd5HashFunc());
    }
}