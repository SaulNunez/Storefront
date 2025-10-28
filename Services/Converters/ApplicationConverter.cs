using Storefront.Models;
using Storefront.Models.DAO;

namespace Storefront.Services.Converters;
public static class ApplicationConverter
{
    public static ApplicationDao ToDao(this Application application)
    {
        return new ApplicationDao
        {
            Name = application.Name,
            Description = application.Description,
            PhotoUrls = application.PhotoUrls
        };
    }
}