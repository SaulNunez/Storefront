using Microsoft.AspNetCore.Identity;

namespace Storefront.Models;

public class Comment
{
    public Guid Id { get; set; }
    public required string UserId { get; set; }
    public IdentityUser? User { get; set; }
    public required string Content { get; set; }
    public Guid ApplicationId { get; set; }
}