using Storefront.Models;

namespace Storefront.Repositories;

public abstract class BasePlatformRepository <R, V>
    where R: IRelease
    where V: IVariant
{
    public abstract Task<R> CreateRelease(Guid applicationId, R releaseEntity);
    public abstract R? GetRelease(Guid id);
    public abstract V CreateVariant(V variant, Guid releaseId);
    public abstract V? GetVariant(Guid macOsVariantId);
    public abstract bool DeleteVariant(Guid variantId);
    public abstract IEnumerable<R> GetReleases(Guid applicationId, int skip, int take);
}