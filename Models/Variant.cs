using Storefront.Models.Enums;

namespace Storefront.Models;

public class Variant
{
    public Guid Id { get; set; }
    public Guid ReleaseId { get; set; }
    public Release? Release { get; set; }
    public required string ObjectKeyInStorage { get; set; }
    public CpuArchitecture CpuPlatform { get; set; } = CpuArchitecture.Universal;
    
    // Optional / Platform-specific fields
    public AndroidScreenDensity? ScreenDensity { get; set; }
    public string? MinOsVersion { get; set; }
    public string? Language { get; set; }
    public long FileSizeBytes { get; set; }
}
