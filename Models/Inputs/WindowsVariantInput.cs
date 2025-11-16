using Storefront.Models.Enums;

namespace Storefront.Models.Inputs;
public record WindowsVariantInput
{
    public WindowsCpuPlatform TargetPlatform { get; init;}
    public string ClientFileName { get; init;}
}