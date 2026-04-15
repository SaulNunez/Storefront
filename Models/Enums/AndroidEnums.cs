namespace Storefront.Models.Enums;

public enum AndroidCpuPlatform
{
    All,
    armeabi_v7a,
    amr_64_v8a,
    x86,
    x86_64
}

public enum AndroidScreenDensity
{
    /// <summary>
    /// Represents apps that are not specific to any screen density. This is used for apps that are designed to work on all screen densities.
    /// </summary>
    nodpi,
    ldpi,
    mdpi,
    hdpi,
    xhdpi,
    xxhdpi,
    xxxhdpi
}