namespace Storefront.Models;

public interface IRelease
{
    /// <summary>
    /// Identifier for a release.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// When was this release created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    /// <summary>
    /// How to identify this version. Developer could follow a variety of strategies like major-minor, version control hash or release date.
    /// </summary>
    public string VersionId { get; set; }
    /// <summary>
    /// Notes for a release. Could describe new features, bug fixes or other information relevant to users.s
    /// </summary>
    public string ReleaseNotes { get; set; }
}