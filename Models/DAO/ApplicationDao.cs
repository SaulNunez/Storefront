using System.Linq.Expressions;

namespace Storefront.Models.DAO;

public record ApplicationDao
{
    public required Guid ApplicationId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required List<string> PhotoUrls { get; init; }
    public List<CommentDao> Comments { get; init; } = [];
    public AndroidApplicationDao? Android { get; init; }
    public WindowsApplicationDao? Windows { get; init; }
    public MacOsApplicationDao? MacOS { get; init; }
    public required string StoreIcon { get; init; }
    public string ElevatorPitch { get; init; }

    public readonly static Expression<Func<Application, ApplicationDao>> MapFromEntity = application => new ApplicationDao
        {
            ApplicationId = application.Id,
            Name = application.Name,
            Description = application.Description,
            PhotoUrls = application.PhotoUrls,
            Android = application.AndroidPackageName?.Length > 0 ? new AndroidApplicationDao
            {
                PackageName = application.AndroidPackageName
            } : null,
            MacOS = application.MacOsReleases.Count > 0 ? new MacOsApplicationDao
            {

            } : null,
            Windows = application.WindowsReleases.Count > 0 ? new WindowsApplicationDao
            {
                
            } : null,
            StoreIcon = application.StoreIconUrl,
            ElevatorPitch = application.ElevatorPitch
        };

    public readonly static Func<Application, ApplicationDao> FromEntity =  MapFromEntity.Compile();
}