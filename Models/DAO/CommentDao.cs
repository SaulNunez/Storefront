namespace Storefront.Models.DAO;

public record CommentDao
{
    public required UserDao User { get; init; }
    public required string Comment { get; init; }
}