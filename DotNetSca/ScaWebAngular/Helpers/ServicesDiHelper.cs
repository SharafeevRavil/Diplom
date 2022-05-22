using ScaWebAngular.Services;

namespace ScaWebAngular.Helpers;

public static class ServicesDiHelper
{
    public static void RegisterServices(IServiceCollection builderServices)
    {
        builderServices.AddScoped<ProjectsService>();
    }
}