using ScaWebAngular.Dto.Reports;

namespace ScaWebAngular.Dto.Projects;

public class ProjectWithReportsDto : ProjectDto
{
    public List<ReportDto> Reports { get; set; }
}