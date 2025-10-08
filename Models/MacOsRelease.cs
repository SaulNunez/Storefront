namespace Storefront.Models;

public record MacOsRelease : IRelease
{
    public Guid Id { get; set; }
    /// <summary>
    /// Version of MacOs, this should allow for subversion filtering or even uploading classic Mac Os executables.
    /// </summary>
    public required float MinimumVersion { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public required string VersionId { get; set; }
    public List<MacOsVariant> Variants { get; set; } = [];
}