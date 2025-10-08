namespace Storefront.Models.DAO;

public record MacOsApplicationDao
{
    public List<MacOsReleaseDao> Releases { get; init; } = [];
}