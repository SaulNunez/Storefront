
using Storefront.Models.DAO;
using Storefront.Repositories;
using Storefront.Services.Converters;

namespace Storefront.Services;

public interface IAppCategoryService
{
    IEnumerable<AppCategoriesDao> GetAllAppCategories();
}

public class AppCategoryService(IAppCategoryRepository appCategoryRepository) : IAppCategoryService
{
    public IEnumerable<AppCategoriesDao> GetAllAppCategories()
    {
        var categories = appCategoryRepository.GetAllAppCategories().ToList();

        return categories.Select(c => c.ToDao());
    }
}