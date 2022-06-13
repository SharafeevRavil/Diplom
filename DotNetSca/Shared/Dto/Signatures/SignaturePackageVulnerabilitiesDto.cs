using Shared.Dto.Packages;

namespace Shared.Dto.Signatures;

public class SignaturePackageVulnerabilitiesDto : SignatureDescriptionDto
{
    public List<PackageVulnerabilitiesDescriptionDto> Packages { get; set; }
}