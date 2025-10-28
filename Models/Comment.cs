namespace Storefront.Models;

public class Comment
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Content { get; set; }
    public Guid ApplicationId { get; set; }
}