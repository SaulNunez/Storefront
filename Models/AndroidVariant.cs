using Storefront.Models.Enums;

namespace Storefront.Models;

/// <summary>
/// Represents a variant of an Android release. Each variant corresponds to a specific CPU platform, language, and/or screen density.
/// Commonly on Android, there might be multiple variants for accomodating different screen densities.
/// </summary>
public class AndroidVariant : IVariant
{
    public Guid Id { get; set; }
    public required string ObjectKeyInStorage { get; set; }
    public string? Language { get; set; }
    public AndroidScreenDensity ScreenDensity { get; set; } = AndroidScreenDensity.nodpi;
    public AndroidCpuPlatform CpuPlatform { get; set; }
}