using Storefront.Models;

namespace Storefront.Repositories;

public class AndroidRepository(StorefrontDbContext dbContext): BasePlatformRepository<AndroidRelease, AndroidVariant>
{
    public override async Task<AndroidRelease> CreateRelease(Guid applicationId, AndroidRelease releaseEntity)
    {
        var application = await dbContext.Applications.FindAsync(applicationId) ?? throw new ArgumentException($"Application with ID {applicationId} not found.", nameof(applicationId));
        application.AndroidReleases.Add(releaseEntity);
        await dbContext.SaveChangesAsync();

        return releaseEntity;
    }

    public override AndroidRelease? GetRelease(Guid id)
    {
        return dbContext.AndroidReleases.Find(id);
    }

    public override AndroidVariant CreateVariant(AndroidVariant variant, Guid releaseId)
    {
        var release = dbContext.AndroidReleases.Find(releaseId) ?? throw new ArgumentException($"Release with ID {releaseId} not found.", nameof(releaseId));
        release?.Variants.Add(variant);
        dbContext.SaveChanges();

        return variant;
    }

    public override AndroidVariant? GetVariant(Guid variantId)
    {
        return dbContext.AndroidVariants.Find(variantId);
    }

    public override bool DeleteVariant(Guid variantId)
    {
        var variant = dbContext.AndroidVariants.Find(variantId);
        if (variant == null)
        {
            return false;
        }
        dbContext.AndroidVariants.Remove(variant);
        return true;
    }

    public override IEnumerable<AndroidRelease> GetReleases(Guid applicationId, int skip, int take)
    {
        return dbContext.AndroidReleases.Skip(skip).Take(take);
    }
}