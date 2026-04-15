
using System.Linq.Expressions;

namespace Storefront.Models.DAO;
public record MacOsReleaseDao : IReleaseDao
{
    /// <summary>
    /// Version of MacOs, this should allow for subversion filtering or even uploading classic Mac Os executables.
    /// </summary>
    public required float MinimumVersion { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public required string VersionId { get; init; }
    public required string ReleaseNotes { get; init; }

    public static readonly Expression<Func<MacOsRelease, MacOsReleaseDao>> MapFromEntity = release => new MacOsReleaseDao
    {
        VersionId = release.VersionId,
        CreatedAt = release.CreatedAt,
        MinimumVersion = release.MinimumVersion,
        ReleaseNotes = release.ReleaseNotes
    };
}