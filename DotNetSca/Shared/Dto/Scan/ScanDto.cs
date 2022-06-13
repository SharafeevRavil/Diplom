using Shared.Dto.Packages;
using Shared.Dto.Signatures;

namespace Shared.Dto.Scan;

public class ScanDto
{
    public List<PackageDto> ExplicitPackages { get; set; }
    public List<SignatureDescriptionDto> Signatures { get; set; }
    public int ProjectId { get; set; }
}