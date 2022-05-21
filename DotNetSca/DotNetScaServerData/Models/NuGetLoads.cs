namespace DotNetScaServerData.Models;

public class NuGetLoads
{
    public int Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public int LoadCount { get; set; }
}