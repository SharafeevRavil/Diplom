using Shared.Dto.Packages;
using Shared.Dto.Signatures;

namespace Shared.Dto.Reports;

public class ReportWithDataDto : ReportDto
{
    public List<SignaturePackageVulnerabilitiesDto> GeneratedSignatures { get; set; }
    public List<PackageVulnerabilitiesDescriptionDto> ExplicitPackages { get; set; }
}