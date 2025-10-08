namespace Storefront.Models.DAO;

public record ApplicationDao
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required List<string> PhotoUrls { get; init; }
    public AndroidApplicationDao? AndroidPackages { get; init; }
    public WindowsApplicationDao? WindowsApplication { get; init; }
    public MacOsApplicationDao? MacOsApplication { get; init; }
    public List<CommentDao> Comments { get; init; } = [];
}