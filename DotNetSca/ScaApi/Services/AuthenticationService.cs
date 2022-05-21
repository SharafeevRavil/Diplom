using System.Security.Cryptography;
using DotNetScaServerData.DbContext;
using DotNetScaServerData.Models;
using Microsoft.EntityFrameworkCore;

namespace ScaApi.Services;

public class AuthenticationService
{
    private readonly ApplicationDbContext _applicationDbContext;

    public AuthenticationService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<string> GenerateAuthHash()
    {
        var md5 = MD5.Create().ComputeHash(Guid.NewGuid().ToByteArray());
        var userAuthentication = new UserAuthentication(DateTimeOffset.UtcNow,BitConverter.ToString(md5).Replace("-",""));
        _applicationDbContext.UserAuthentications.Add(userAuthentication);
        await _applicationDbContext.SaveChangesAsync();
        return userAuthentication.AuthHash;
    }

    public async Task<bool> CheckAuthenticated(string authHash)
    {
        return await _applicationDbContext.UserAuthentications.AnyAsync(x => x.AuthHash == authHash);
    }
}