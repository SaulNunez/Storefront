using Storefront.Models.Enums;

namespace Storefront.Models.DAO;

public record MacOsVariantDao
{
    public MacOSPlatforms SupportedPlatform { get; init; }
    public required string ContentLocation { get; init; }
}