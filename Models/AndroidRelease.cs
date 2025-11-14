namespace Storefront.Models;

public class AndroidRelease : IRelease
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public required string VersionId { get; set; }
    public required List<AndroidVariant> Variants { get; set; }
    public string Changes { get; set; }
}