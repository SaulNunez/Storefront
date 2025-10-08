namespace Storefront.Models;

public class Comment
{
    public Guid Id { get; set; }
    public Guid UserId { get; init; }
    public required string Content { get; init; }
}