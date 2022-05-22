using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaApi.Services;
using Shared.Dto;

namespace ScaApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly PackagesService _packagesService;
    private readonly VulnerabilitiesService _vulnerabilitiesService;

    public TestController(ILogger<TestController> logger, PackagesService packagesService, VulnerabilitiesService vulnerabilitiesService)
    {
        _logger = logger;
        _packagesService = packagesService;
        _vulnerabilitiesService = vulnerabilitiesService;
    }

    [HttpGet("TestHash")]
    public IActionResult TestHash(string hash, int distS, int limit)
    {
        return Ok(_packagesService.FindByHash(hash, distS, limit));
    }

    [HttpPost("TestSignatures")]
    //[Authorize]
    public async Task<IActionResult> TestSignatures([FromBody]IEnumerable<SignatureDto> signatures)
    {
        return Ok(await _packagesService.FindVulnerabilitiesInSignatures(signatures));
    }

    [HttpPost("TestPackages")]
    //[Authorize]
    public async Task<IActionResult> TestPackages([FromBody]IEnumerable<PackageDto> packages)
    {
        return Ok(await _vulnerabilitiesService.FindVulnerabilities(packages));
    }
}