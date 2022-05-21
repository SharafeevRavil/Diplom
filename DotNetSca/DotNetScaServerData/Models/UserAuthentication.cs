namespace DotNetScaServerData.Models;

public class UserAuthentication
{
    public int Id { get; set; }
    public DateTimeOffset GeneratedDate { get; set; }
    public string AuthHash { get; set; }

    public UserAuthentication(DateTimeOffset generatedDate, string authHash)
    {
        GeneratedDate = generatedDate;
        AuthHash = authHash;
    }
}