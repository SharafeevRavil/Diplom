using OssIndexClient;
using Shared.Dto;

namespace ScaApi.Services;

public class VulnerabilitiesService
{
    public async Task<List<PackageWithVulnerabilitiesDto>> FindVulnerabilities(IEnumerable<PackageDto> packages)
    {
        var groups = packages.Chunk(128);
        var list = new List<PackageWithVulnerabilitiesDto>();

        foreach (var packageGroup in groups)
        {
            var client = new OssIndex();
            var reports = await client.GetReports(packageGroup
                .Select(x => new Package(EcoSystem.nuget, x.PackageId, x.PackageVersion)));

            foreach (var report in reports)
            {
                list.Add(new PackageWithVulnerabilitiesDto()
                {
                    PackageId = report.Name,
                    PackageVersion = report.Version,
                    Vulnerabilities = report.Vulnerabilities
                        .Select(v => new Shared.Dto.VulnerabilityDto()
                        {
                            Title = v.Title,
                            Description = v.Description,
                            Cve = v.Cve,
                            Cwe = v.Cwe,
                            CvssScore = v.CvssScore,
                            CvssVector = v.CvssVector,
                            Reference = v.Reference,
                        }).ToList()
                });
            }
        }

        return list;
    }
}
