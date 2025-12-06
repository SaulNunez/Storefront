using System.Linq.Expressions;

namespace Storefront.Models.DAO;

public record AppCategoriesDao (int Id, string Name)
{
    public readonly static Expression<Func<AppCategories, AppCategoriesDao>> MapFromEntity = entity => new AppCategoriesDao (entity.Id, entity.Name);
}