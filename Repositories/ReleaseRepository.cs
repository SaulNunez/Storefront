using Microsoft.EntityFrameworkCore;
using Storefront.Models;
using Storefront.Models.Enums;

namespace Storefront.Repositories;

public interface IReleaseRepository
{
    Task<Release> CreateReleaseAsync(Guid applicationId, Release release);
    Task<Release?> GetReleaseAsync(Guid releaseId);
    Task<IEnumerable<Release>> GetReleasesForAppAsync(Guid applicationId, TargetPlatform? platform = null, int skip = 0, int take = 10);
    Task<bool> DeleteReleaseAsync(Guid releaseId);
    Task<Variant> CreateVariantAsync(Guid releaseId, Variant variant);
    Task<Variant?> GetVariantAsync(Guid variantId);
    Task<bool> DeleteVariantAsync(Guid variantId);
}

public class ReleaseRepository(StorefrontDbContext dbContext) : IReleaseRepository
{
    public async Task<Release> CreateReleaseAsync(Guid applicationId, Release release)
    {
        var application = await dbContext.Applications.FindAsync(applicationId) 
            ?? throw new ArgumentException($"Application with ID {applicationId} not found.", nameof(applicationId));
        
        release.ApplicationId = applicationId;
        dbContext.Releases.Add(release);
        await dbContext.SaveChangesAsync();
        return release;
    }

    public async Task<Release?> GetReleaseAsync(Guid releaseId)
    {
        return await dbContext.Releases
            .Include(r => r.Variants)
            .FirstOrDefaultAsync(r => r.Id == releaseId);
    }

    public async Task<IEnumerable<Release>> GetReleasesForAppAsync(Guid applicationId, TargetPlatform? platform = null, int skip = 0, int take = 10)
    {
        var query = dbContext.Releases
            .Include(r => r.Variants)
            .Where(r => r.ApplicationId == applicationId);

        if (platform.HasValue)
        {
            query = query.Where(r => r.Platform == platform.Value);
        }

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<bool> DeleteReleaseAsync(Guid releaseId)
    {
        var release = await dbContext.Releases.FindAsync(releaseId);
        if (release == null) return false;

        dbContext.Releases.Remove(release);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Variant> CreateVariantAsync(Guid releaseId, Variant variant)
    {
        var release = await dbContext.Releases
            .Include(r => r.Variants)
            .FirstOrDefaultAsync(r => r.Id == releaseId)
            ?? throw new ArgumentException($"Release with ID {releaseId} not found.", nameof(releaseId));

        variant.ReleaseId = releaseId;
        release.Variants.Add(variant);
        await dbContext.SaveChangesAsync();
        return variant;
    }

    public async Task<Variant?> GetVariantAsync(Guid variantId)
    {
        return await dbContext.Variants.FindAsync(variantId);
    }

    public async Task<bool> DeleteVariantAsync(Guid variantId)
    {
        var variant = await dbContext.Variants.FindAsync(variantId);
        if (variant == null) return false;

        dbContext.Variants.Remove(variant);
        await dbContext.SaveChangesAsync();
        return true;
    }
}
