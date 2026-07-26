using System.Linq.Expressions;
using Storefront.Models.Enums;

namespace Storefront.Models.DAO;

public record VariantDao
{
    public Guid Id { get; init; }
    public required string ObjectKeyInStorage { get; init; }
    public CpuArchitecture CpuPlatform { get; init; }
    public AndroidScreenDensity? ScreenDensity { get; init; }
    public string? MinOsVersion { get; init; }
    public string? Language { get; init; }
    public long FileSizeBytes { get; init; }

    public static readonly Expression<Func<Variant, VariantDao>> MapFromEntity = variant => new VariantDao
    {
        Id = variant.Id,
        ObjectKeyInStorage = variant.ObjectKeyInStorage,
        CpuPlatform = variant.CpuPlatform,
        ScreenDensity = variant.ScreenDensity,
        MinOsVersion = variant.MinOsVersion,
        Language = variant.Language,
        FileSizeBytes = variant.FileSizeBytes
    };

    public static readonly Func<Variant, VariantDao> FromEntity = MapFromEntity.Compile();
}

public record ReleaseDao
{
    public Guid Id { get; init; }
    public Guid ApplicationId { get; init; }
    public TargetPlatform Platform { get; init; }
    public required string VersionId { get; init; }
    public required string ReleaseNotes { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public List<VariantDao> Variants { get; init; } = [];

    public static readonly Expression<Func<Release, ReleaseDao>> MapFromEntity = release => new ReleaseDao
    {
        Id = release.Id,
        ApplicationId = release.ApplicationId,
        Platform = release.Platform,
        VersionId = release.VersionId,
        ReleaseNotes = release.ReleaseNotes,
        CreatedAt = release.CreatedAt,
        Variants = release.Variants.Select(v => new VariantDao
        {
            Id = v.Id,
            ObjectKeyInStorage = v.ObjectKeyInStorage,
            CpuPlatform = v.CpuPlatform,
            ScreenDensity = v.ScreenDensity,
            MinOsVersion = v.MinOsVersion,
            Language = v.Language,
            FileSizeBytes = v.FileSizeBytes
        }).ToList()
    };

    public static readonly Func<Release, ReleaseDao> FromEntity = MapFromEntity.Compile();
}
