using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaWebAngular.Services;

namespace ScaWebAngular.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly ProjectsService _projectsService;

    public ProjectsController(ProjectsService projectsService)
    {
        _projectsService = projectsService;
    }
    
    [HttpGet("TestIndexPls")]
    public IActionResult Index()
    {
        return Ok("aaaaa");
    }
}