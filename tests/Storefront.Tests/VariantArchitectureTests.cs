using Storefront.Models;
using Storefront.Models.Enums;
using Storefront.Models.Inputs;
using Storefront.Repositories;
using Storefront.Services;

namespace Storefront.Tests;

public class VariantArchitectureTests
{
    [Theory]
    [InlineData(CpuArchitecture.PowerPC)]
    [InlineData(CpuArchitecture.M68k)]
    [InlineData(CpuArchitecture.Universal)]
    public async Task Mac_accepts_vintage_and_universal_cpus(CpuArchitecture cpu)
    {
        var (service, release) = Setup(TargetPlatform.MacOs);

        var variant = await service.CreateVariantAsync(release.Id, Input(cpu));

        Assert.Equal([cpu], variant.CpuArchitectures);
    }

    [Fact]
    public async Task Mac_accepts_a_powerpc_intel_universal_binary()
    {
        var (service, release) = Setup(TargetPlatform.MacOs);

        var variant = await service.CreateVariantAsync(release.Id, Input(CpuArchitecture.PowerPC, CpuArchitecture.X86));

        Assert.Equal([CpuArchitecture.PowerPC, CpuArchitecture.X86], variant.CpuArchitectures);
    }

    [Theory]
    [InlineData(TargetPlatform.Windows, CpuArchitecture.PowerPC)]
    [InlineData(TargetPlatform.Android, CpuArchitecture.M68k)]
    public async Task Other_platforms_reject_mac_only_cpus(TargetPlatform platform, CpuArchitecture cpu)
    {
        var (service, release) = Setup(platform);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateVariantAsync(release.Id, Input(cpu)));
    }

    [Fact]
    public async Task Universal_cannot_be_combined_with_specific_cpus()
    {
        var (service, release) = Setup(TargetPlatform.MacOs);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateVariantAsync(release.Id, Input(CpuArchitecture.Universal, CpuArchitecture.Arm64)));
    }

    [Fact]
    public async Task Empty_cpu_list_is_rejected()
    {
        var (service, release) = Setup(TargetPlatform.MacOs);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateVariantAsync(release.Id, Input()));
    }

    [Fact]
    public async Task Selection_skips_a_powerpc_intel_build_for_apple_silicon_and_prefers_the_thinnest_match()
    {
        var (service, release) = Setup(TargetPlatform.MacOs);
        var ppcIntel = await service.CreateVariantAsync(release.Id, Input(CpuArchitecture.PowerPC, CpuArchitecture.X86));
        var universal2 = await service.CreateVariantAsync(release.Id, Input(CpuArchitecture.X64, CpuArchitecture.Arm64));
        var arm64 = await service.CreateVariantAsync(release.Id, Input(CpuArchitecture.Arm64));
        var m68k = await service.CreateVariantAsync(release.Id, Input(CpuArchitecture.M68k));

        Assert.Equal(arm64.Id, (await service.SelectBestVariantAsync(release.Id, CpuArchitecture.Arm64))?.Id);
        Assert.Equal(universal2.Id, (await service.SelectBestVariantAsync(release.Id, CpuArchitecture.X64))?.Id);
        Assert.Equal(ppcIntel.Id, (await service.SelectBestVariantAsync(release.Id, CpuArchitecture.PowerPC))?.Id);
        Assert.Equal(m68k.Id, (await service.SelectBestVariantAsync(release.Id, CpuArchitecture.M68k))?.Id);
    }

    private static VariantInput Input(params CpuArchitecture[] cpus) => new()
    {
        ObjectKeyInStorage = "key",
        CpuArchitectures = [.. cpus],
    };

    private static (ReleaseService, Release) Setup(TargetPlatform platform)
    {
        var release = new Release { Id = Guid.NewGuid(), Platform = platform, VersionId = "1.0", ReleaseNotes = "" };
        return (new ReleaseService(new InMemoryReleaseRepository(release)), release);
    }

    private sealed class InMemoryReleaseRepository(Release release) : IReleaseRepository
    {
        public Task<Release?> GetReleaseAsync(Guid releaseId) => Task.FromResult(releaseId == release.Id ? release : null);

        public Task<Variant> CreateVariantAsync(Guid releaseId, Variant variant)
        {
            variant.Id = Guid.NewGuid();
            release.Variants.Add(variant);
            return Task.FromResult(variant);
        }

        public Task<Release> CreateReleaseAsync(Guid applicationId, Release r) => throw new NotSupportedException();
        public Task<IEnumerable<Release>> GetReleasesForAppAsync(Guid applicationId, TargetPlatform? platform = null, int skip = 0, int take = 10) => throw new NotSupportedException();
        public Task<bool> DeleteReleaseAsync(Guid releaseId) => throw new NotSupportedException();
        public Task<Variant?> GetVariantAsync(Guid variantId) => throw new NotSupportedException();
        public Task<bool> DeleteVariantAsync(Guid variantId) => throw new NotSupportedException();
    }
}
