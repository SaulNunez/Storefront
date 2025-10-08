using Enums;

namespace Storefront.Models.DAO;

public record AndroidVariantDao: IContent
{
    public required string ContentLocation { get; init; }
    public string? Language { get; set; }
    public string? ScreenDensity { get; set; }
    public AndroidCpuPlatform TargetCPU { get; init; }
}