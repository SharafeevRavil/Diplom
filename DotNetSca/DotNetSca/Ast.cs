using System.Text;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace DotNetSca;

public class Ast
{
    public static async Task<IEnumerable<(Compilation, SyntaxTree)>> GetSyntaxTreesFromSolution(string solutionPath)
    {
        MSBuildLocator.RegisterDefaults();

        using var workspace = MSBuildWorkspace.Create();

        var solution = await workspace.OpenSolutionAsync(solutionPath);
        var syntaxTrees = new List<(Compilation, SyntaxTree)>();
        foreach (var project in solution.Projects)
        {
            ////Console.WriteLine($"Documents: {project.Documents.Count()}");

            var compilation = await project.GetCompilationAsync();

            syntaxTrees.AddRange(compilation.SyntaxTrees.Select(x => (compilation, x)));
            //var semanticModel = compilation.GetSemanticModel(compilation.SyntaxTrees.ToList()[0]);
            //var a = compilation.SyntaxTrees.ToList()[0].GetCompilationUnitRoot().;
            //var q = semanticModel.LookupSymbols(210).Where(x => x);
        }

        return syntaxTrees;
    }

    public static async Task<IEnumerable<(Compilation, SyntaxTree)>> GetSyntaxTreesFromFile(string filePath)
    {
        var programText = await File.ReadAllTextAsync(filePath);
        var tree = CSharpSyntaxTree.ParseText(programText);
        var root = tree.GetCompilationUnitRoot();

        var syntaxTrees = root.Members
            .Select(x => x.SyntaxTree)
            .ToList();
        //Compilation q = CSharpCompilation.Create("decompiled", syntaxTrees);

        //return syntaxTrees.Select(x => (q, x));
        return syntaxTrees.Select(x => ((Compilation)CSharpCompilation.Create("q", new[] { x }), x));
    }

    public static async Task<IEnumerable<FeaturesList>> GetFeaturesList(IEnumerable<(Compilation, SyntaxTree)> syntaxTrees)
    {
        var list = new List<FeaturesList>();
        foreach (var (compilation, syntaxTree) in syntaxTrees)
        {
            ////Console.WriteLine(syntaxTree.FilePath);
            var methods = (await syntaxTree.GetRootAsync())
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .ToList();
            /*var methods2 = (syntaxTree.GetCompilationUnitRoot())
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .ToList();*/

            ////Console.WriteLine($"Methods: {methods.Count}");
            if (methods.Count <= 0) continue;

            var semModel = compilation.GetSemanticModel(syntaxTree);
            foreach (var method in methods)
            {
                /*//
                node.DescendantNodes()
                    .SelectMany(x => new []{
                        (SyntaxNode)(x as IdentifierNameSyntax)
                        ?? (x as VariableDeclaratorSyntax) 
                    })
                    .Where(x => x != null)
                    //.Select(x => x.Parent)
                    .ToList()
                //*/
                //var q = method
                //var tokens = method.DescendantTokens();
                ////Console.WriteLine($"Method: {method.Identifier}");
                //var sb = new StringBuilder();
                var methodFeature = new FeaturesList()
                {
                    Description = GetMethodDescription(method)
                };
                if (method.Body != null)
                {
                    
                    foreach (var node in method.Body.ChildNodes())
                    {
                        var tokens2 = node.DescendantTokens().Where(x => x.IsKind(SyntaxKind.IdentifierToken));
                        var curNodeStr = new StringBuilder(node.ToString());
                        var spanStart = node.Span.Start;
                        var q = semModel.GetTypeInfo(node);
                        foreach (var innerNode in node.DescendantNodes().Reverse())
                        {
                            // ReSharper disable once ConvertIfStatementToSwitchStatement
                            if (innerNode is IdentifierNameSyntax idNameSyntax)
                            {
                                var info = semModel.GetSymbolInfo(idNameSyntax);
                                if (info.Symbol is ILocalSymbol or IParameterSymbol)
                                {
                                    var token = idNameSyntax.Identifier;
                                    curNodeStr.Remove(token.Span.Start - spanStart, token.Span.Length);
                                    curNodeStr.Insert(token.Span.Start - spanStart, "ID");
                                }

                                continue;
                            }

                            // ReSharper disable once InvertIf
                            if (innerNode is VariableDeclaratorSyntax varDeclSyntax)
                            {
                                var token = varDeclSyntax.Identifier;
                                curNodeStr.Remove(token.Span.Start - spanStart, token.Span.Length);
                                curNodeStr.Insert(token.Span.Start - spanStart, "ID");
                                continue;
                            }
                        }

                        /*foreach (var token in tokens2.Reverse())
                        {
                            curNodeStr.Remove(token.Span.Start - spanStart, token.Span.Length);
                            curNodeStr.Insert(token.Span.Start - spanStart, "ID");
                            //spanStart -= (token.Span.Length - "ID".Length);
                        }*/
                        
                        //sb.Append($"{curNodeStr}\n");
                        methodFeature.Features.AddRange(curNodeStr.ToString()
                            .Replace("\r", "")
                            .Replace("\t", "")
                            .Split('\n')
                            .Where(x => x != "{" && x != "}"));
                        ////Console.WriteLine($" *** {node.Kind()}");
                        ////Console.WriteLine($"  1  {node}");
                        ////Console.WriteLine($"  2  {curNodeStr}");
                    }
                    
                }

                //if (sb.Length == 0) continue;
                //sb.Remove(sb.Length - 1, 1); //убрать лишний \n
                //list.Add(sb.ToString());
                
                if(methodFeature.Features.Count == 0) continue;
                list.Add(methodFeature);
            }
        }

        return list;
    }

    private static string GetMethodDescription(MethodDeclarationSyntax method)
    {
        var methodStr = $"[{method.Modifiers} {method.ReturnType} {method.Identifier}{method.ParameterList}]";
        if (method.Parent is not TypeDeclarationSyntax type) 
            return methodStr;

        var typeStr = $"[{type.Modifiers} {type.Keyword} {type.Identifier}]";

        if (type.Parent is not NamespaceDeclarationSyntax ns)
            return $"{typeStr} {methodStr}";

        var nsStr = $"[{ns.NamespaceKeyword} {ns.Name}]";
        
        return $"{nsStr} {typeStr} {methodStr}";
    }

    public class FeaturesList
    {
        public List<string> Features { get; set; } = new List<string>();
        public string Description { get; set; }
        public int SymbolsLength => Features.Sum(x => x.Length);
    }
}