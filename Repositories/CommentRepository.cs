using Storefront.Models;

namespace Storefront.Repositories;

public interface ICommentRepository
{
    IQueryable<Comment> GetApplicationComments(Guid applicationId, int size = 20);
}

public class CommentRepository(StorefrontDbContext dbContext) : ICommentRepository
{
    public IQueryable<Comment> GetApplicationComments(Guid applicationId, int size = 20)
    {
        return dbContext.Comments
            .Where(c => c.ApplicationId == applicationId)
            .Take(size);
    }
}
