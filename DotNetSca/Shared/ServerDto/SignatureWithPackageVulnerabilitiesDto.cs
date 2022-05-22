namespace Shared.Dto;

public class SignatureWithPackageVulnerabilitiesDto : SignatureDto
{
    public List<PackageWithVulnerabilitiesDescriptionDto> Packages { get; set; }
}