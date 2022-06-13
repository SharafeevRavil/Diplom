using Shared.Dto.Reports;

namespace Shared.Dto.Projects;

public class ProjectWithReportsDto : ProjectDto
{
    public List<ReportDto> Reports { get; set; }
}