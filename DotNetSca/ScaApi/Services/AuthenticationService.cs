using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotNetScaServerData.DbContext;
using DotNetScaServerData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ScaApi.Services;

public class AuthenticationService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IConfiguration _configuration;

    public AuthenticationService(ApplicationDbContext applicationDbContext, IConfiguration configuration)
    {
        _applicationDbContext = applicationDbContext;
        _configuration = configuration;
    }

    /* deprecated -> jwt
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
    }*/
    
    public async Task<JwtSecurityToken> GetToken(/*IdentityUser user = null*/)
    {
        var authClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            _configuration["JWT:ValidIssuer"],
            _configuration["JWT:ValidAudience"],
            //expires: DateTime.Now.AddHours(3),
            expires: DateTime.Now.AddYears(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}