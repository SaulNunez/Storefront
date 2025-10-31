using Storefront.Models.Enums;

namespace Storefront.Models;
public class MacOsVariant
{
    public Guid Id { get; set; }
    public required string ContentLocation { get; set; }
    public MacOSPlatforms CpuPlatform { get; set; }
}