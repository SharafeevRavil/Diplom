using DotNetSca.SimHash;
using DotNetScaServerData.DbContext;
using DotNetScaServerData.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Dto.Packages;
using Shared.Dto.Signatures;
using Shared.Dto.Vulnerabilities;

namespace ScaApi.Services;

public class PackagesService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly VulnerabilitiesService _vulnerabilitiesService;

    public const int DefaultDistance = 4;
    public const int HashLength = SimHash.HashLength;

    public PackagesService(ApplicationDbContext applicationDbContext, VulnerabilitiesService vulnerabilitiesService)
    {
        _applicationDbContext = applicationDbContext;
        _vulnerabilitiesService = vulnerabilitiesService;
    }

    public IQueryable<Signature> FindByHash(IEnumerable<byte> hash, int distance = DefaultDistance, int limit = 1) =>
        FindByHash(SimHash.HashBytesToString(hash), distance, limit);

    public IQueryable<Signature> FindByHash(string hash, int distanceSymbols = DefaultDistance, int limit = 1)
    {
        var distance = (double)distanceSymbols / HashLength;

        return _applicationDbContext.Signatures
            .FromSqlInterpolated($@"select distinct on (""PackageId"") *
from (select ""Signatures"".*, hamming_text(""Hash"", {hash}) as dist
      from ""Signatures"") as ""Inner""
where dist > {distance}")
            .Include(x => x.Package)
            .Take(limit);
    }

    public async Task<List<SignaturePackageVulnerabilitiesDto>> FindVulnerabilitiesInSignatures(
        IEnumerable<SignatureDto> signatures)
    {
        var list = signatures
            .Select(x =>
            {
                var found = FindByHash(x.Hash, limit: 5).ToList();
                return new SignaturePackageVulnerabilitiesDto()
                {
                    Id = x.Id,
                    Packages = found.Select(y => new PackageVulnerabilitiesDescriptionDto()
                    {
                        PackageId = y.Package.PackageId,
                        PackageVersion = y.Package.PackageVersion,
                        Description = y.Description
                    }).ToList()
                };
            }).ToList();

        //find vulnerabilities
        var packages = list.SelectMany(x => x.Packages);
        var vulnerabilities = await _vulnerabilitiesService.FindVulnerabilities(packages);
        foreach (var package in list.SelectMany(signature => signature.Packages))
        {
            package.Vulnerabilities = vulnerabilities
                .FirstOrDefault(x => x.PackageId == package.PackageId 
                                     && x.PackageVersion == package.PackageVersion)
                ?.Vulnerabilities ?? new List<VulnerabilityDto>();
        }

        return list;
    }
}