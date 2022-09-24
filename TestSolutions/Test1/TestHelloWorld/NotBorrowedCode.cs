namespace TestHelloWorld;

public class NotBorrowedCode
{
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
    
    public async Task<ReportWithDataDto> GetReport(string userId, int projectId, int reportId)
    {
        var project = _applicationDbContext.Projects
            .Include(x => x.Reports)
            .FirstOrDefault(x => x.Id == projectId);
        if (project == null)
            throw new ArgumentException();

        if (project.Reports.All(x => x.Id != reportId))
            throw new ArgumentException();

        var report = await _applicationDbContext.Reports
            .Include(x => x.Initiator)
            .Include(x => x.ExplicitPackages)
            .ThenInclude(x => x.Vulnerabilities)
            .Include(x => x.GeneratedSignatures)
            .ThenInclude(x => x.FoundPackages)
            .ThenInclude(x => x.Vulnerabilities)
            .FirstOrDefaultAsync(x => x.Id == reportId);

        if (report == null)
            throw new ArgumentException();

        return new ReportWithDataDto()
        {
            Id = report.Id,
            Initiator = report.Initiator.Email,
            CreatedDate = report.CreatedDate,
            ExplicitPackages = report.ExplicitPackages
                .Select(x => new PackageVulnerabilitiesDescriptionDto()
                {
                    Description = x.Description,
                    Vulnerabilities = x.Vulnerabilities
                        .Select(y => new VulnerabilityDto()
                        {
                            Cve = y.Cve,
                            Cwe = y.Cwe,
                            Description = y.Description,
                            Reference = y.Reference,
                            Title = y.Title,
                            CvssScore = y.CvssScore,
                            CvssVector = y.CvssVector
                        }).ToList(),
                    PackageId = x.PackageId,
                    PackageVersion = x.PackageVersion
                }).ToList(),
            GeneratedSignatures = report.GeneratedSignatures
                .Select(s => new SignaturePackageVulnerabilitiesDto()
                {
                    Id = s.Id,
                    Hash = s.Hash,
                    Description = s.Description,
                    Packages = s.FoundPackages
                        .Select(x => new PackageVulnerabilitiesDescriptionDto()
                        {
                            Description = x.Description,
                            Vulnerabilities = x.Vulnerabilities
                                .Select(y => new VulnerabilityDto()
                                {
                                    Cve = y.Cve,
                                    Cwe = y.Cwe,
                                    Description = y.Description,
                                    Reference = y.Reference,
                                    Title = y.Title,
                                    CvssScore = y.CvssScore,
                                    CvssVector = y.CvssVector
                                }).ToList(),
                            PackageId = x.PackageId,
                            PackageVersion = x.PackageVersion
                        }).ToList()
                }).ToList(),
            IsFilledByServer = report.IsFilledByServer,
        };
    }
}