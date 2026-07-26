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
        var variant = new Variant
        {
            ReleaseId = releaseId,
            ObjectKeyInStorage = input.ObjectKeyInStorage,
            CpuPlatform = input.CpuPlatform,
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
            .Where(v => v.CpuPlatform == CpuArchitecture.Universal || v.CpuPlatform == clientCpu)
            .Where(v => !clientDensity.HasValue || v.ScreenDensity == null || v.ScreenDensity == clientDensity.Value || v.ScreenDensity == AndroidScreenDensity.nodpi)
            .OrderByDescending(v => v.CpuPlatform == clientCpu)
            .ThenByDescending(v => v.ScreenDensity == clientDensity)
            .FirstOrDefault();

        return matchingVariant != null ? VariantDao.FromEntity(matchingVariant) : null;
    }
}
