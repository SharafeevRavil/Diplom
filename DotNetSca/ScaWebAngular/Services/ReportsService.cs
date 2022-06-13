using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScaWebAngular.Data;
using ScaWebAngular.Models;
using Shared.Dto.Check;
using Shared.Dto.Packages;
using Shared.Dto.Reports;
using Shared.Dto.Scan;
using Shared.Dto.Signatures;
using Shared.Dto.Vulnerabilities;

namespace ScaWebAngular.Services;

public class ReportsService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public ReportsService(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _applicationDbContext = applicationDbContext;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task Submit(ScanDto scanDto, string userId)
    {
        var project = _applicationDbContext.Projects
            .Include(x => x.Reports)
            .FirstOrDefault(x => x.Id == scanDto.ProjectId);
        if (project == null)
            throw new ArgumentException();


        var user = await _userManager.FindByIdAsync(userId);
        var report = new Report()
        {
            Initiator = user,
            CreatedDate = DateTimeOffset.UtcNow,
            ExplicitPackages = scanDto.ExplicitPackages
                .Select(x => new Package()
                {
                    PackageId = x.PackageId,
                    PackageVersion = x.PackageVersion
                })
                .ToList(),
            GeneratedSignatures = scanDto.Signatures
                .Select(x => new Signature()
                {
                    Hash = x.Hash,
                    Description = x.Description
                })
                .ToList(),
            IsFilledByServer = false
        };

        project.Reports.Add(report);
        await _applicationDbContext.Reports.AddAsync(report);
        await _applicationDbContext.SaveChangesAsync();

        //load signature data
        var vendorHost = _configuration["Vendor:Host"];
        var vendorToken = _configuration["Vendor:Token"];

        var checkDto = new CheckDto()
        {
            Packages = report.ExplicitPackages
                .Select(x => new PackageDto()
                {
                    PackageId = x.PackageId,
                    PackageVersion = x.PackageVersion
                }).ToList(),
            Signatures = report.GeneratedSignatures
                .Select(x => new SignatureDto()
                {
                    Id = x.Id,
                    Hash = x.Hash
                }).ToList()
        };

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", vendorToken);
        var checkResponseDto = await httpClient.PostAsJsonAsync($"{vendorHost}/Check/CheckVulnerabilities", checkDto);
        var response = await checkResponseDto.Content.ReadFromJsonAsync<CheckResponseDto>();
        if (response == null)
            return;

        foreach (var p in response.VulnerabilitiesInPackages)
        {
            var p2 = report.ExplicitPackages.FirstOrDefault(x =>
                x.PackageId == p.PackageId && x.PackageVersion == p.PackageVersion);
            if (p2 == null) continue;

            p2.Vulnerabilities = p.Vulnerabilities
                .Select(x => new Vulnerability()
                {
                    Cve = x.Cve,
                    Cwe = x.Cwe,
                    Description = x.Description,
                    Reference = x.Reference,
                    Title = x.Title,
                    CvssScore = x.CvssScore,
                    CvssVector = x.CvssVector
                }).ToList();
        }

        foreach (var s in response.VulnerabilitiesInSignatures)
        {
            var s2 = report.GeneratedSignatures.FirstOrDefault(x => x.Id == s.Id);
            if (s2 == null) continue;

            s2.FoundPackages = s.Packages
                .Select(x => new Package()
                {
                    Description = x.Description,
                    Vulnerabilities = x.Vulnerabilities
                        .Select(y => new Vulnerability()
                        {
                            Cve = y.Cve,
                            Cwe = y.Cwe,
                            Description = y.Description,
                            Reference = y.Reference,
                            Title = y.Title,
                            CvssScore = y.CvssScore,
                            CvssVector = y.CvssVector
                        }).ToList(),
                    PackageId = x.PackageId,
                    PackageVersion = x.PackageVersion
                }).ToList();
        }

        report.IsFilledByServer = true;

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task<ReportWithDataDto> GetReport(string userId, int projectId, int reportId)
    {
        var project = _applicationDbContext.Projects
            .Include(x => x.Reports)
            .FirstOrDefault(x => x.Id == projectId);
        if (project == null)
            throw new ArgumentException();

        if (project.Reports.All(x => x.Id != reportId))
            throw new ArgumentException();

        var report = await _applicationDbContext.Reports
            .Include(x => x.Initiator)
            .Include(x => x.ExplicitPackages)
            .ThenInclude(x => x.Vulnerabilities)
            .Include(x => x.GeneratedSignatures)
            .ThenInclude(x => x.FoundPackages)
            .ThenInclude(x => x.Vulnerabilities)
            .FirstOrDefaultAsync(x => x.Id == reportId);

        if (report == null)
            throw new ArgumentException();

        return new ReportWithDataDto()
        {
            Id = report.Id,
            Initiator = report.Initiator.Email,
            CreatedDate = report.CreatedDate,
            ExplicitPackages = report.ExplicitPackages
                .Select(x => new PackageVulnerabilitiesDescriptionDto()
                {
                    Description = x.Description,
                    Vulnerabilities = x.Vulnerabilities
                        .Select(y => new VulnerabilityDto()
                        {
                            Cve = y.Cve,
                            Cwe = y.Cwe,
                            Description = y.Description,
                            Reference = y.Reference,
                            Title = y.Title,
                            CvssScore = y.CvssScore,
                            CvssVector = y.CvssVector
                        }).ToList(),
                    PackageId = x.PackageId,
                    PackageVersion = x.PackageVersion
                }).ToList(),
            GeneratedSignatures = report.GeneratedSignatures
                .Select(s => new SignaturePackageVulnerabilitiesDto()
                {
                    Id = s.Id,
                    Hash = s.Hash,
                    Description = s.Description,
                    Packages = s.FoundPackages
                        .Select(x => new PackageVulnerabilitiesDescriptionDto()
                        {
                            Description = x.Description,
                            Vulnerabilities = x.Vulnerabilities
                                .Select(y => new VulnerabilityDto()
                                {
                                    Cve = y.Cve,
                                    Cwe = y.Cwe,
                                    Description = y.Description,
                                    Reference = y.Reference,
                                    Title = y.Title,
                                    CvssScore = y.CvssScore,
                                    CvssVector = y.CvssVector
                                }).ToList(),
                            PackageId = x.PackageId,
                            PackageVersion = x.PackageVersion
                        }).ToList()
                }).ToList(),
            IsFilledByServer = report.IsFilledByServer,
        };
    }
}