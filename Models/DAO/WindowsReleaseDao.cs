using System.Linq.Expressions;
using Storefront.Models.Enums;

namespace Storefront.Models.DAO;

public record WindowsReleaseDao
{
    public DateTimeOffset CreatedAt { get; init; }
    public required string VersionId { get; init; }

    public List<WindowsApplicationVariantDao> Variants { get; init; } = [];

    public static readonly Expression<Func<WindowsRelease, WindowsReleaseDao>> MapFromEntity = release => new WindowsReleaseDao
    {
        VersionId = release.VersionId,
        CreatedAt = release.CreatedAt
    };
}