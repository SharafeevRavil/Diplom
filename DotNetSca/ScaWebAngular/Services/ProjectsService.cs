using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScaWebAngular.Data;
using ScaWebAngular.Dto.Projects;
using ScaWebAngular.Dto.Reports;
using ScaWebAngular.Helpers;
using ScaWebAngular.Models;

namespace ScaWebAngular.Services;

public class ProjectsService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ProjectsService(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _applicationDbContext = applicationDbContext;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /*public async Task<bool> CheckAdminAccess(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains(DatabaseHelper.RoleNames.First(x => x == "Admin"));
    }*/

    public async Task<List<ProjectDto>> GetProjects(string userId, bool isAdmin = false)
    {
        IQueryable<Project> query = _applicationDbContext.Projects
            .Include(x => x.AllowedUsers);

        //if (!await CheckAdminAccess(userId))
        if (!isAdmin)
            query = query.Where(x => x.AllowedUsers.Any(y => y.Id == userId));

        return query.Select(x => new ProjectDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            })
            .ToList();
    }

    public async Task<ProjectDto> CreateProject(string userId, ProjectDto dto)
    {
        /*if (!await CheckAdminAccess(userId))
            throw new UnauthorizedAccessException();*/

        var project = new Project()
        {
            Description = dto.Description,
            Name = dto.Name,
            AllowedUsers = new List<ApplicationUser>()
            {
                await _userManager.FindByIdAsync(userId)
            }
        };
        _applicationDbContext.Projects.Add(project);
        await _applicationDbContext.SaveChangesAsync();
        dto.Id = project.Id;

        return dto;
    }

    public async Task<ProjectWithReportsDto> GetProject(int id, string userId, bool isAdmin = false)
    {
        var q = _applicationDbContext.Projects
            .Include(x => x.Reports)
            .ThenInclude(y => y.Initiator)
            .Include(x => x.AllowedUsers)
            .Where(x => x.Id == id);
        if (!isAdmin)
            q = q.Where(x => x.AllowedUsers.Any(y => y.Id == userId));

        return q.Select(x => new ProjectWithReportsDto()
        {
            Id = x.Id,
            Description = x.Description,
            Name = x.Name,
            Reports = x.Reports
                .Select(y => new ReportDto()
                {
                    Id = y.Id,
                    Initiator = y.Initiator.UserName,
                    CreatedDate = y.CreatedDate,
                    IsFilledByServer = y.IsFilledByServer
                }).ToList()
        }).FirstOrDefault();
    }
}