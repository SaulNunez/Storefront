using Enums;

namespace Storefront.Models;

public class AndroidVariant
{
    public Guid Id { get; set; }
    public required string ContentLocation { get; set; }
    public string? Language { get; set; }
    public string? ScreenDensity { get; set; }
    public AndroidCpuPlatform TargetCPU { get; set; }
}