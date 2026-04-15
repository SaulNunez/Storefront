
namespace Storefront.Models.DAO;

public interface IReleaseDao
{
    public DateTimeOffset CreatedAt { get; init; }
    public string VersionId { get; init; }
    public string ReleaseNotes { get; init; }
}