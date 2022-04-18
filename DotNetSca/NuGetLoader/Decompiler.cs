using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;

namespace NuGetLoader;

public class Decompiler
{
    public static async Task Decompile(string dllPath)
    {
        var decompiler = new CSharpDecompiler(dllPath, new DecompilerSettings());
        var a = decompiler.DecompileWholeModuleAsString();
        await File.WriteAllTextAsync($"{dllPath}.cs", a);
    }
}