using Shared.Dto.Packages;
using Shared.Dto.Signatures;

namespace Shared.Dto.Check;

public class CheckDto
{
    public List<SignatureDto> Signatures { get; set; }
    public List<PackageDto> Packages { get; set; }
}