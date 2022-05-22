using Microsoft.EntityFrameworkCore;
using ScaWebAngular.Data;
using ScaWebAngular.Dto.Projects;

namespace ScaWebAngular.Services;

public class ProjectsService
{
    private readonly ApplicationDbContext _applicationDbContext;

    public ProjectsService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public List<ProjectDto> GetProjects(string userId) =>
        _applicationDbContext
            .Projects
            .Include(x => x.AllowedUsers)
            .Where(x => x.AllowedUsers.Any(y => y.Id == userId))
            .Select(x => new ProjectDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            })
            .ToList();
}