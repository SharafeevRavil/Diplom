using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScaWebAngular.Models;
using ScaWebAngular.Services;
using Shared.Dto.Projects;

namespace ScaWebAngular.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly ProjectsService _projectsService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectsController(ProjectsService projectsService, UserManager<ApplicationUser> userManager)
    {
        _projectsService = projectsService;
        _userManager = userManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = (User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _userManager.FindByIdAsync(userId);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        
        var projects = await _projectsService.GetProjects(userId, isAdmin);
        return Ok(projects);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var userId = (User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _userManager.FindByIdAsync(userId);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        
        var project = await _projectsService.GetProject(id, userId, isAdmin);
        return Ok(project);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(ProjectDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        /*var user = await _userManager.FindByIdAsync(userId);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        if (!isAdmin) return Unauthorized();*/
        
        var created = await _projectsService.CreateProject(userId, dto);
        return Ok(created);
    }
}