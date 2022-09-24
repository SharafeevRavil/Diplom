using ScaApi.OssIndexClient;
using Shared.Dto.Packages;
using Shared.Dto.Vulnerabilities;

namespace ScaApi.Services;

public class VulnerabilitiesService
{
    public async Task<List<PackageVulnerabilitiesDto>> FindVulnerabilities(IEnumerable<PackageDto> packages)
    {
        //var groups = packages.Chunk(128);
        var groups = packages.Chunk(10);
        var list = new List<PackageVulnerabilitiesDto>();

        foreach (var packageGroup in groups)
        {
            var client = new OssIndex();
            var reports = await client.GetReports(packageGroup
                .Select(x => new Package(EcoSystem.nuget, x.PackageId, x.PackageVersion)));

            foreach (var report in reports)
            {
                list.Add(new PackageVulnerabilitiesDto()
                {
                    PackageId = report.Name,
                    PackageVersion = report.Version,
                    Vulnerabilities = report.Vulnerabilities
                        .Select(v => new VulnerabilityDto()
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
