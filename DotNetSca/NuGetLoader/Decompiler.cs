using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;

namespace NuGetLoader;

public class Decompiler
{
    public static async Task<string> Decompile(string dllPath)
    {
        try
        {
            var decompiler = new CSharpDecompiler(dllPath,
                new UniversalAssemblyResolver(dllPath, false, null),
                new DecompilerSettings
                {
                    ThrowOnAssemblyResolveErrors = false,
                });
            var code = decompiler.DecompileWholeModuleAsString();
            var path = $"{dllPath}.cs";
            await File.WriteAllTextAsync(path, code);
            return path;
        }
        catch (Exception e)
        {
            Console.WriteLine($"EXCEPTION {e}");
            return null;
        }
    }
}