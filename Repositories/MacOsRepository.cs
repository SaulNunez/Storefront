using Storefront.Models;
using Storefront.Repositories;

public class MacOsRepository(StorefrontDbContext dbContext) : BasePlatformRepository<MacOsRelease, MacOsVariant>
{
    public override async Task<MacOsRelease> CreateRelease(Guid applicationId, MacOsRelease releaseEntity)
    {
        var application = await dbContext.Applications.FindAsync(applicationId) ?? throw new ArgumentException($"Application with ID {applicationId} not found.", nameof(applicationId));
        application.MacOsReleases.Add(releaseEntity);
        await dbContext.SaveChangesAsync();

        return releaseEntity;
    }

    public override MacOsRelease? GetRelease(Guid id)
    {
        return dbContext.MacOsReleases.Find(id);
    }

    public override MacOsVariant CreateVariant(MacOsVariant variant, Guid releaseId)
    {
        var release = dbContext.MacOsReleases.Find(releaseId) ?? throw new ArgumentException($"Release with ID {releaseId} not found.", nameof(releaseId));
        release?.Variants.Add(variant);
        dbContext.SaveChanges();

        return variant;
    }

    public override MacOsVariant? GetVariant(Guid macOsVariantId)
    {
        return dbContext.MacOsVariants.Find(macOsVariantId);
    }

    public override bool DeleteVariant(Guid variantId)
    {
        var variant = dbContext.WindowsVariants.Find(variantId);
        if (variant == null)
        {
            return false;
        }
        dbContext.WindowsVariants.Remove(variant);
        return true;
    }

    public override IEnumerable<MacOsRelease> GetReleases(Guid applicationId, int skip, int take)
    {
        var application = dbContext.Applications.Find(applicationId) ?? throw new ArgumentException($"Application with ID {applicationId} not found.", nameof(applicationId));
        return application.MacOsReleases.Skip(skip).Take(take);
    }
}