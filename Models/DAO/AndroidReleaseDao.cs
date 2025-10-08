namespace Storefront.Models.DAO;

public record AndroidReleaseDao
{
    public DateTimeOffset CreatedAt { get; init; }
    public required string VersionId { get; init; }
    public List<AndroidVariantDao> Variants { get; init; } = [];
}