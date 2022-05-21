using System.Diagnostics;

namespace DotNetSca;

public static class SimHashPython
{
    private static string RunCmd(string pythonPath, string pythonFile, string args)
    {
        var start = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{pythonFile}\" \"{args}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        using var process = Process.Start(start);
        using var reader = process.StandardOutput;
        var stderr = process.StandardError.ReadToEnd();
        var result = reader.ReadToEnd();
        return result;
    }

    public static string GetSymHash(string input) =>
        RunCmd(@"C:/Users/Admin/AppData/Local/Programs/Python/Python39/python.exe",
            @"C:/Users/Admin/RiderProjects/Diplom/SimHashPython/main.py", input)
            .Trim();
}