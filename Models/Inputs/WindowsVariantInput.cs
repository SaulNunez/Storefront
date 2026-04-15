using Storefront.Models.Enums;

namespace Storefront.Models.Inputs;
public record WindowsVariantInput : IVariantInput
{
    public WindowsCpuPlatform TargetPlatform { get; init;}
    public required string ClientFileName { get; init;}
}