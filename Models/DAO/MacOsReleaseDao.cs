
using System.Linq.Expressions;

namespace Storefront.Models.DAO;
public record MacOsReleaseDao
{
    /// <summary>
    /// Version of MacOs, this should allow for subversion filtering or even uploading classic Mac Os executables.
    /// </summary>
    public required float MinimumVersion { get; init; }
    public DateTimeOffset CreatedAt { get; set; }
    public required string VersionId { get; set; }

    public static readonly Expression<Func<MacOsRelease, MacOsReleaseDao>> MapFromEntity = release => new MacOsReleaseDao
    {
        VersionId = release.VersionId,
        CreatedAt = release.CreatedAt,
        MinimumVersion = release.MinimumVersion
    };
}