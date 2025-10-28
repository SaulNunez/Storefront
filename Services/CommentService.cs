using Microsoft.EntityFrameworkCore;
using Storefront.Models;
using Storefront.Models.DAO;

namespace Storefront.Services;
public class CommentService(StorefrontDbContext dbContext)
{
    public CommentDao GetApplicationComments(Guid applicationId)
    {
        var comments = dbContext.Comments.Where(c => c.ApplicationId = applicationId);
    }
}