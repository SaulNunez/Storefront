using System.Linq.Expressions;
using Storefront.Models.Enums;

namespace Storefront.Models.DAO;

public record ApplicationDao
{
    public required Guid ApplicationId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required List<string> PhotoUrls { get; init; }
    public List<CommentDao> Comments { get; init; } = [];
    public List<TargetPlatform> SupportedPlatforms { get; init; } = [];
    public List<ReleaseDao> Releases { get; init; } = [];
    public string? StoreIcon { get; init; }
    public string ElevatorPitch { get; init; }

    public readonly static Expression<Func<Application, ApplicationDao>> MapFromEntity = application => new ApplicationDao
    {
        ApplicationId = application.Id,
        Name = application.Name,
        Description = application.Description,
        PhotoUrls = application.PhotoUrls,
        SupportedPlatforms = application.Releases.Select(r => r.Platform).Distinct().ToList(),
        Releases = application.Releases.Select(r => new ReleaseDao
        {
            Id = r.Id,
            ApplicationId = r.ApplicationId,
            Platform = r.Platform,
            VersionId = r.VersionId,
            ReleaseNotes = r.ReleaseNotes,
            CreatedAt = r.CreatedAt,
            Variants = r.Variants.Select(v => new VariantDao
            {
                Id = v.Id,
                ObjectKeyInStorage = v.ObjectKeyInStorage,
                CpuPlatform = v.CpuPlatform,
                ScreenDensity = v.ScreenDensity,
                MinOsVersion = v.MinOsVersion,
                Language = v.Language,
                FileSizeBytes = v.FileSizeBytes
            }).ToList()
        }).ToList(),
        StoreIcon = application.StoreIconUrl,
        ElevatorPitch = application.ElevatorPitch
    };

    public readonly static Func<Application, ApplicationDao> FromEntity = MapFromEntity.Compile();
}