using Storefront.Models.Enums;

namespace Storefront.Models.DAO;

public record WindowsApplicationVariant : IContent
{
    public Guid Id { get; set; }
    public required string ContentLocation { get; init; }
    public WindowsCpuPlatform CpuPlatform { get; init; }
}