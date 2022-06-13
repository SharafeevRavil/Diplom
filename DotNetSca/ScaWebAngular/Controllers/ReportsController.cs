using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaWebAngular.Services;
using Shared.Dto.Scan;

namespace ScaWebAngular.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportsService _reportsService;

    public ReportsController(ReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    [HttpPost("Submit")]
    public async Task<IActionResult> Submit(ScanDto scanDto)
    {
        var userId = (User.FindFirstValue(ClaimTypes.NameIdentifier));
        await _reportsService.Submit(scanDto, userId);

        return Ok();
    }

    [HttpGet("/api/projects/{projectId:int}/reports/{reportId:int}")]
    public async Task<IActionResult> GetReport(int projectId, int reportId)
    {
        var userId = (User.FindFirstValue(ClaimTypes.NameIdentifier));
        var report = await _reportsService.GetReport(userId, projectId, reportId);

        return Ok(report);
    }
}