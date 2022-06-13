using Shared.Dto.Packages;
using Shared.Dto.Signatures;

namespace Shared.Dto.Check;

public class CheckResponseDto
{
    public List<SignaturePackageVulnerabilitiesDto> VulnerabilitiesInSignatures { get; set; }
    public List<PackageVulnerabilitiesDto> VulnerabilitiesInPackages { get; set; }
}