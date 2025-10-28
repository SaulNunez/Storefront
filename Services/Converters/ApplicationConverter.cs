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
            PhotoUrls = application.PhotoUrls,
            Android = application.AndroidPackageName.Count() > 0 ? new AndroidApplicationDao
            {
                PackageName = application.AndroidPackageName
            } : null,
            MacOS = application.MacOsReleases.Count() > 0 ? new MacOsApplicationDao
            {

            } : null,
            Windows = application.WindowsReleases.Count() > 0 ? new WindowsApplicationDao
            {
                
            } : null
        };
    }
}