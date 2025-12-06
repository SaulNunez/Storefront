
using Microsoft.EntityFrameworkCore;
using Storefront.Models.DAO;
using Storefront.Repositories;

namespace Storefront.Services;

public interface IAppCategoryService
{
    IEnumerable<AppCategoriesDao> GetAllAppCategories();
}

public class AppCategoryService(IAppCategoryRepository appCategoryRepository) : IAppCategoryService
{
    public IEnumerable<AppCategoriesDao> GetAllAppCategories()
    {
        var categories = appCategoryRepository.GetAllAppCategories().Select(AppCategoriesDao.MapFromEntity);
        return categories.AsEnumerable();
    }
}