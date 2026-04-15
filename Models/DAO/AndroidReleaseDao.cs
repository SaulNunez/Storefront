using System.Linq.Expressions;

namespace Storefront.Models.DAO;

public record AndroidReleaseDao: IReleaseDao
{
    public DateTimeOffset CreatedAt { get; init; }
    public required string VersionId { get; init; }
    public List<AndroidVariantDao> Variants { get; init; } = [];
    public required string ReleaseNotes { get; init; }

    public static readonly Expression<Func<AndroidRelease, AndroidReleaseDao>> MapFromEntity = release => new AndroidReleaseDao
    {
        VersionId = release.VersionId,
        CreatedAt = release.CreatedAt,
        ReleaseNotes = release.ReleaseNotes,
    };
}