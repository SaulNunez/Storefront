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
}