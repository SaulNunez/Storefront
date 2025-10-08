namespace Storefront.Models.DAO;

public record AndroidApplicationDao
{
    public required string PackageName { get; init; }
    public required List<AndroidReleaseDao> Releases { get; init; }
}