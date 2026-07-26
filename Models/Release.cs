using Storefront.Models.Enums;

namespace Storefront.Models;

public class Release
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public Application? Application { get; set; }
    public TargetPlatform Platform { get; set; }
    public required string VersionId { get; set; }
    public required string ReleaseNotes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<Variant> Variants { get; set; } = [];
}
