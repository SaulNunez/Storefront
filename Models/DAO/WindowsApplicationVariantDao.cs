using Storefront.Models.Enums;

namespace Storefront.Models.DAO;

public record WindowsApplicationVariantDao : IContent
{
    public required string ContentLocation { get; init; }
    public WindowsCpuPlatform CpuPlatform { get; init; }
}