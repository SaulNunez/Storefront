using Storefront.Models.Enums;

namespace Storefront.Models;

public class Variant
{
    public Guid Id { get; set; }
    public Guid ReleaseId { get; set; }
    public Release? Release { get; set; }
    public required string ObjectKeyInStorage { get; set; }
    /// <summary>
    /// Every CPU this binary runs on natively. A fat binary lists several, e.g. a Mac Universal 2 build is
    /// [X64, Arm64] and a PowerPC/Intel Universal build is [PowerPC, X86].
    /// </summary>
    public List<CpuArchitecture> CpuArchitectures { get; set; } = [CpuArchitecture.Universal];
    
    // Optional / Platform-specific fields
    public AndroidScreenDensity? ScreenDensity { get; set; }
    public string? MinOsVersion { get; set; }
    public string? Language { get; set; }
    public long FileSizeBytes { get; set; }
}
