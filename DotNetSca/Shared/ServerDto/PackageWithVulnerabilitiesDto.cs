namespace Shared.Dto;

public class PackageWithVulnerabilitiesDto : PackageDto
{
    public List<VulnerabilityDto> Vulnerabilities { get; set; }
}

public class PackageWithVulnerabilitiesDescriptionDto : PackageWithVulnerabilitiesDto
{
    public string Description { get; set; }
}