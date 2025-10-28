using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Storefront.Models;
using Storefront.Models.DAO;
using Storefront.Services.Converters;

namespace Storefront.Services;
public class CommentService(StorefrontDbContext dbContext)
{
    public List<CommentDao> GetApplicationComments(Guid applicationId, int size = 20)
    {
        var comments = dbContext.Comments.Where(c => c.ApplicationId == applicationId).Take(size);
        return [.. comments.Select(c => c.ToDao())];
    }
}