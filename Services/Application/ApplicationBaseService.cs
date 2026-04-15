using Storefront.Models;
using Storefront.Models.DAO;
using Storefront.Models.Inputs;

namespace Storefront.Services.Application;

public abstract class BaseApplicationService<R, C, V>
    where R: IReleaseDao
    where C: IReleaseInput
    where V: IVariantInput
{
    public abstract Task<string> CreateVariantUploadLink(Guid variantId);
    public abstract Task<string> CreateVariantDownloadLink(Guid variantId);
    public abstract Task<Guid> CreateVariant(Guid applicationId, Guid releaseId, V variantInput);
    public abstract Task<R> GetRelease(Guid releaseId);
    public abstract IEnumerable<R> GetApplicationReleases(Guid applicationRelease, int skip= 0, int take = 10);
    public abstract Task<R> CreateRelease(Guid applicationId, C releaseCreateInput);
}