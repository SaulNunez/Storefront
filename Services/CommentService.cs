using Storefront.Models.DAO;
using Storefront.Repositories;

namespace Storefront.Services;

public interface ICommentService
{
    List<CommentDao> GetApplicationComments(Guid applicationId, int size = 20);
}

public class CommentService(ICommentRepository commentRepository) : ICommentService
{
    public List<CommentDao> GetApplicationComments(Guid applicationId, int size = 20)
    {
        return commentRepository.GetApplicationComments(applicationId, size).Select(CommentDao.MapFromEntity).ToList();
    }
}
