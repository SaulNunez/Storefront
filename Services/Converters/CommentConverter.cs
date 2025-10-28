using Storefront.Models;
using Storefront.Models.DAO;

namespace Storefront.Services.Converters;
public static class CommentConverter
{
    public static CommentDao ToDao(this Comment comment)
    {
        return new CommentDao
        {
            User = new UserDao
            {
                UserName = "Joe Doe"
            },
            Comment = comment.Content
        };
    }
}