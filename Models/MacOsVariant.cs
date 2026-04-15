using Storefront.Models.Enums;

namespace Storefront.Models;

/// <summary>
/// Represents a variant of a MacOS release. Each variant corresponds to a specific CPU platform.
/// </summary>
public class MacOsVariant : IVariant
{
    public Guid Id { get; set; }
    public required string ObjectKeyInStorage { get; set; }
    public MacOSPlatforms CpuPlatform { get; set; }
}