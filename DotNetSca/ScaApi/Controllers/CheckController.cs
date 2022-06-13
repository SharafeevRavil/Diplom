using Microsoft.AspNetCore.Mvc;
using ScaApi.Services;
using Shared.Dto.Check;
using Shared.Dto.Packages;
using Shared.Dto.Signatures;

namespace ScaApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CheckController : ControllerBase
{
    private readonly PackagesService _packagesService;
    private readonly VulnerabilitiesService _vulnerabilitiesService;

    public CheckController(PackagesService packagesService, VulnerabilitiesService vulnerabilitiesService)
    {
        _packagesService = packagesService;
        _vulnerabilitiesService = vulnerabilitiesService;
    }

    [HttpPost("CheckVulnerabilities")]
    //[Authorize]
    public async Task<IActionResult> CheckVulnerabilities([FromBody] CheckDto checkDto)
    {
        var s = await _packagesService.FindVulnerabilitiesInSignatures(checkDto.Signatures);
        var p = await _vulnerabilitiesService.FindVulnerabilities(checkDto.Packages);
        var checkResponseDto = new CheckResponseDto()
        {
            VulnerabilitiesInPackages = p,
            VulnerabilitiesInSignatures = s
        };
        return Ok(checkResponseDto);
    }
}