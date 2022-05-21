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

    [HttpGet(Name = "GenerateAuthHash")]
    public async Task<IActionResult> GenerateAuthHash()
    {
        return Ok(await _authenticationService.GenerateAuthHash());
    }
}