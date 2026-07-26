using Storefront.Models.Enums;

namespace Storefront.Models.Inputs;

public record ReleaseInput
{
    public required TargetPlatform Platform { get; init; }
    public required string VersionId { get; init; }
    public required string ReleaseNotes { get; init; }
}

public record VariantInput
{
    public required string ObjectKeyInStorage { get; init; }
    public CpuArchitecture CpuPlatform { get; init; } = CpuArchitecture.Universal;
    public AndroidScreenDensity? ScreenDensity { get; init; }
    public string? MinOsVersion { get; init; }
    public string? Language { get; init; }
    public long FileSizeBytes { get; init; }
}
