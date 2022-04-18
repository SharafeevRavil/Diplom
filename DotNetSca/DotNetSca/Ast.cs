using System.Text;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace DotNetSCA;

public class Test
{
    public static async Task<IEnumerable<SyntaxTree>> GetSyntaxTreesFromSolution(string solutionPath)
    {
        MSBuildLocator.RegisterDefaults();

        using var workspace = MSBuildWorkspace.Create();

        var solution = await workspace.OpenSolutionAsync(solutionPath);
        var syntaxTrees = new List<SyntaxTree>();
        foreach (var project in solution.Projects)
        {
            Console.WriteLine($"Documents: {project.Documents.Count()}");

            var compilation = await project.GetCompilationAsync();

            syntaxTrees.AddRange(compilation.SyntaxTrees);
        }

        return syntaxTrees;
    }

    public static async Task<IEnumerable<SyntaxTree>> GetSyntaxTreesFromFile(string filePath)
    {
        var programText = await File.ReadAllTextAsync(filePath);
        var tree = CSharpSyntaxTree.ParseText(programText);
        var root = tree.GetCompilationUnitRoot();

        return root.Members
            .Select(x => x.SyntaxTree);
    }

    public static async Task<IEnumerable<string>> GetFeatures(IEnumerable<SyntaxTree> syntaxTrees)
    {
        var list = new List<string>();
        foreach (var tree in syntaxTrees)
        {
            Console.WriteLine(tree.FilePath);
            var methods = (await tree.GetRootAsync())
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .ToList();

            Console.WriteLine($"Methods: {methods.Count}");
            if (methods.Count <= 0) continue;

            foreach (var method in methods)
            {
                var sb = new StringBuilder();
                Console.WriteLine($"Method: {method.Identifier}");
                foreach (var node in method.Body.ChildNodes())
                {
                    sb.Append($"{node}\n");
                    Console.WriteLine($" *** {node.Kind()}");
                    Console.WriteLine($"     {node}");
                }

                sb.Remove(sb.Length - 1, 1); //убрать лишний \n
                list.Add(sb.ToString());
            }
        }

        return list;
    }
}