using Microsoft.AspNetCore.Identity;

namespace Storefront.Models;

public class Application
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string ElevatorPitch { get; set; }
    public required List<string> PhotoUrls { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public List<AndroidRelease> AndroidReleases { get; set; } = [];
    public List<WindowsRelease> WindowsReleases { get; set; } = [];
    public List<MacOsRelease> MacOsReleases { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
    public string? AndroidPackageName { get; set; }
    public IdentityUser? Owner { get; set; }
    public string OwnerId { get; set;}
    public string StoreIconUrl {get; set; }
    public AppCategories Category { get; set; }
    public int CategoryId { get; set; }
}