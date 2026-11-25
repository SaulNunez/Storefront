using Storefront.Models;

namespace Storefront.Repositories;

public interface IAppCategoryRepository
{
    IQueryable<AppCategories> GetAllAppCategories();
    bool CategoryExists(int categoryId);
}

public class AppCategoryRepository(StorefrontDbContext dbContext) : IAppCategoryRepository
{
    public IQueryable<AppCategories> GetAllAppCategories()
    {
        return dbContext.Categories.OrderBy(x => x.Name);
    }

    public bool CategoryExists(int categoryId)
    {
        return dbContext.Categories.Any(x => x.Id == categoryId);
    }
}
