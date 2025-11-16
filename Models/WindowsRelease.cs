using Storefront.Models.DAO;
using Storefront.Models.Enums;

namespace Storefront.Models;

public class WindowsRelease : IRelease
{
    public Guid Id { get; set; }
    public List<WindowsVariant> Variants { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public required string VersionId { get; set; }
    public string Changes { get; set; }
}