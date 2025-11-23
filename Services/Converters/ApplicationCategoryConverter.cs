using Storefront.Models;
using Storefront.Models.DAO;

namespace Storefront.Services.Converters;
public static class ApplicationCategoryConverter
{
    public static AppCategoriesDao ToDao(this AppCategories category)
    {
        return new AppCategoriesDao(category.Id, category.Name);
    }
}