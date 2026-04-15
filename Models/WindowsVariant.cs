using Storefront.Models.Enums;

namespace Storefront.Models;

/// <summary>
/// Represents a variant of a Windows release. Each variant corresponds to a specific CPU platform.
public class WindowsVariant : IVariant
{
    public Guid Id { get; set; }
    public required string ObjectKeyInStorage { get; set; }
    public WindowsCpuPlatform CpuPlatform { get; set; }
}