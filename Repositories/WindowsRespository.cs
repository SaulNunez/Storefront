using Storefront.Models;

namespace Storefront.Repositories;

public class WindowsRepository(StorefrontDbContext dbContext) : BasePlatformRepository<WindowsRelease, WindowsVariant>
{
    public override async Task<WindowsRelease> CreateRelease(Guid applicationId, WindowsRelease releaseEntity)
    {
        var application = await dbContext.Applications.FindAsync(applicationId) ?? throw new ArgumentException($"Application with ID {applicationId} not found.", nameof(applicationId));
        application.WindowsReleases.Add(releaseEntity);
        await dbContext.SaveChangesAsync();

        return releaseEntity;
    }

    public override WindowsRelease? GetRelease(Guid id)
    {
        return dbContext.WindowsReleases.Find(id) ?? throw new ArgumentException($"Application with ID {id} not found.", nameof(id));
    }

    public override WindowsVariant CreateVariant(WindowsVariant variant, Guid releaseId)
    {
        var release = dbContext.WindowsReleases.Find(releaseId) ?? throw new ArgumentException($"Release with ID {releaseId} not found.", nameof(releaseId));
        release?.Variants.Add(variant);
        dbContext.SaveChanges();

        return variant;
    }

    public override WindowsVariant? GetVariant(Guid variantId)
    {
        return dbContext.WindowsVariants.Find(variantId);
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

    public override IEnumerable<WindowsRelease> GetReleases(Guid applicationId, int skip, int take)
    {
        var application = dbContext.Applications.Find(applicationId) ?? throw new ArgumentException($"Application with ID {applicationId} not found.", nameof(applicationId));
        return application.WindowsReleases.Skip(skip).Take(take);
    }
}