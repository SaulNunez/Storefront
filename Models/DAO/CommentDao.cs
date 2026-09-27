using System.Linq.Expressions;

namespace Storefront.Models.DAO;

public record CommentDao
{
    public required UserDao User { get; init; }
    public required string Comment { get; init; }

    public readonly static Expression<Func<Comment, CommentDao>> MapFromEntity = comment =>  new CommentDao
        {
            User = new UserDao
            {
                UserName = comment.User != null && comment.User.UserName != null ? comment.User.UserName : "Unknown user"
            },
            Comment = comment.Content
        };
}