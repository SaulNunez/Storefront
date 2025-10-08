namespace Storefront.Models;

public interface IRelease
{
    public DateTimeOffset CreatedAt { get; set; }
    /// <summary>
    /// How to identify this version. Developer could follow a variety of strategies like major-minor, version control hash or release date.
    /// </summary>
    public string VersionId { get; set; }
    public string Changes { get; set; }
}