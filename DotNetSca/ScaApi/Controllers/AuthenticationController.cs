using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using ScaApi.Services;

namespace ScaApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;

    public AuthenticationController(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpGet(Name = "GenerateJwtToken")]
    public async Task<IActionResult> GenerateJwtToken()
    {
        var token = await _authenticationService.GetToken();
        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(tokenStr);
    }
}