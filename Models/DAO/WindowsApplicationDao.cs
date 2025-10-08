using Storefront.Models.Enums;

namespace Storefront.Models.DAO;

public record WindowsApplicationDao
{
    public List<WindowsReleaseDao> Releases { get; init; } = [];
}