using Storefront.Models;

namespace Storefront.Repositories;

public interface IAppCategoryRepository
{
    IQueryable<AppCategories> GetAllAppCategories();
}

public class AppCategoryRepository(StorefrontDbContext dbContext) : IAppCategoryRepository
{
    public IQueryable<AppCategories> GetAllAppCategories()
    {
        return dbContext.Categories.OrderBy(x => x.Name);
    }
}