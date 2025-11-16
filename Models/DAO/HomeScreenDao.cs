namespace Storefront.Models.DAO;

public record HomeScreenDao
{
    public required List<ApplicationDao> MostPopularApplications { get; init; }
    public required List<ApplicationDao> LatestApplications { get; init; }
}