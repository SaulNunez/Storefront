using Storefront.Models;
using Storefront.Models.DAO;
using Storefront.Models.Enums;
using Storefront.Models.Exceptions;
using Storefront.Models.Inputs;
using Storefront.Repositories;

namespace Storefront.Services;

public interface IReleaseService
{
    Task<ReleaseDao> CreateReleaseAsync(Guid applicationId, ReleaseInput input);
    Task<ReleaseDao> GetReleaseAsync(Guid releaseId);
    Task<List<ReleaseDao>> GetReleasesForAppAsync(Guid applicationId, TargetPlatform? platform = null, int skip = 0, int take = 10);
    Task<bool> DeleteReleaseAsync(Guid releaseId);
    Task<VariantDao> CreateVariantAsync(Guid releaseId, VariantInput input);
    Task<VariantDao> GetVariantAsync(Guid variantId);
    Task<bool> DeleteVariantAsync(Guid variantId);
    Task<VariantDao?> SelectBestVariantAsync(Guid releaseId, CpuArchitecture clientCpu, AndroidScreenDensity? clientDensity = null);
}

public class ReleaseService(IReleaseRepository releaseRepository) : IReleaseService
{
    public async Task<ReleaseDao> CreateReleaseAsync(Guid applicationId, ReleaseInput input)
    {
        var release = new Release
        {
            ApplicationId = applicationId,
            Platform = input.Platform,
            VersionId = input.VersionId,
            ReleaseNotes = input.ReleaseNotes,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var created = await releaseRepository.CreateReleaseAsync(applicationId, release);
        return ReleaseDao.FromEntity(created);
    }

    public async Task<ReleaseDao> GetReleaseAsync(Guid releaseId)
    {
        var release = await releaseRepository.GetReleaseAsync(releaseId)
            ?? throw new NotFoundException($"Release with ID {releaseId} not found.");
        return ReleaseDao.FromEntity(release);
    }

    public async Task<List<ReleaseDao>> GetReleasesForAppAsync(Guid applicationId, TargetPlatform? platform = null, int skip = 0, int take = 10)
    {
        var releases = await releaseRepository.GetReleasesForAppAsync(applicationId, platform, skip, take);
        return releases.Select(ReleaseDao.FromEntity).ToList();
    }

    public async Task<bool> DeleteReleaseAsync(Guid releaseId)
    {
        return await releaseRepository.DeleteReleaseAsync(releaseId);
    }

    public async Task<VariantDao> CreateVariantAsync(Guid releaseId, VariantInput input)
    {
        var release = await releaseRepository.GetReleaseAsync(releaseId)
            ?? throw new ArgumentException($"Release with ID {releaseId} not found.", nameof(releaseId));
        ValidateCpuArchitectures(release.Platform, input.CpuArchitectures);

        var variant = new Variant
        {
            ReleaseId = releaseId,
            ObjectKeyInStorage = input.ObjectKeyInStorage,
            CpuArchitectures = input.CpuArchitectures.Distinct().ToList(),
            ScreenDensity = input.ScreenDensity,
            MinOsVersion = input.MinOsVersion,
            Language = input.Language,
            FileSizeBytes = input.FileSizeBytes
        };

        var created = await releaseRepository.CreateVariantAsync(releaseId, variant);
        return VariantDao.FromEntity(created);
    }

    public async Task<VariantDao> GetVariantAsync(Guid variantId)
    {
        var variant = await releaseRepository.GetVariantAsync(variantId)
            ?? throw new NotFoundException($"Variant with ID {variantId} not found.");
        return VariantDao.FromEntity(variant);
    }

    public async Task<bool> DeleteVariantAsync(Guid variantId)
    {
        return await releaseRepository.DeleteVariantAsync(variantId);
    }

    public async Task<VariantDao?> SelectBestVariantAsync(Guid releaseId, CpuArchitecture clientCpu, AndroidScreenDensity? clientDensity = null)
    {
        var release = await releaseRepository.GetReleaseAsync(releaseId)
            ?? throw new NotFoundException($"Release with ID {releaseId} not found.");

        var matchingVariant = release.Variants
            .Where(v => v.CpuArchitectures.Contains(CpuArchitecture.Universal) || v.CpuArchitectures.Contains(clientCpu))
            .Where(v => !clientDensity.HasValue || v.ScreenDensity == null || v.ScreenDensity == clientDensity.Value || v.ScreenDensity == AndroidScreenDensity.nodpi)
            .OrderByDescending(v => v.CpuArchitectures.Contains(clientCpu))
            .ThenByDescending(v => v.ScreenDensity == clientDensity)
            // Among native matches, prefer the thinnest binary (an Arm64-only build over a Universal 2 one).
            .ThenBy(v => v.CpuArchitectures.Count)
            .FirstOrDefault();

        return matchingVariant != null ? VariantDao.FromEntity(matchingVariant) : null;
    }

    /// <summary>
    /// CPUs a variant may target on each platform. Linux is not restricted.
    /// </summary>
    private static readonly Dictionary<TargetPlatform, CpuArchitecture[]> AllowedArchitectures = new()
    {
        [TargetPlatform.Windows] = [CpuArchitecture.Universal, CpuArchitecture.X86, CpuArchitecture.X64, CpuArchitecture.Arm32, CpuArchitecture.Arm64],
        [TargetPlatform.MacOs] = [CpuArchitecture.Universal, CpuArchitecture.Arm64, CpuArchitecture.X64, CpuArchitecture.X86, CpuArchitecture.PowerPC, CpuArchitecture.M68k],
        [TargetPlatform.Android] = [CpuArchitecture.Universal, CpuArchitecture.Arm32, CpuArchitecture.Arm64, CpuArchitecture.X86, CpuArchitecture.X64],
    };

    private static void ValidateCpuArchitectures(TargetPlatform platform, List<CpuArchitecture> architectures)
    {
        if (architectures.Count == 0)
        {
            throw new ArgumentException("A variant must target at least one CPU architecture.");
        }

        if (architectures.Contains(CpuArchitecture.Universal) && architectures.Distinct().Count() > 1)
        {
            throw new ArgumentException("Universal already covers every CPU and can't be combined with specific architectures.");
        }

        if (AllowedArchitectures.TryGetValue(platform, out var allowed))
        {
            var unsupported = architectures.Where(a => !allowed.Contains(a)).Distinct().ToList();
            if (unsupported.Count > 0)
            {
                throw new ArgumentException($"{platform} variants can't target {string.Join(", ", unsupported)}.");
            }
        }
    }
}
