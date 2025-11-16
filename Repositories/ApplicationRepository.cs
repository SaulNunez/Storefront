using Genbox.SimpleS3.AmazonS3;
using Storefront.Models;

namespace Storefront.Repositories;

public interface IApplicationRepository
{
    Application CreateApplication(Application application);
    Application? GetApplication(Guid id);
    IQueryable<Application> GetLatestApplications(int maxLength = 10);
    IQueryable<Application> GetMostPopularApplications(int maxLength = 10);
    AndroidRelease? GetAndroidRelease(Guid id);
    MacOsRelease? GetMacOSRelease(Guid id);
    WindowsRelease? GetWindowsRelease(Guid id);
    MacOsVariant CreateMacOSVariant(MacOsVariant macOsVariant, Guid releaseId);
    WindowsVariant CreateWindowsVariant(WindowsVariant windowsVariant, Guid releaseId);
    AndroidVariant CreateAndroidVariant(AndroidVariant androidVariant, Guid releaseId);
    MacOsVariant? GetMacOSVariant(Guid macOsVariantId);
    AndroidVariant? GetAndroidVariant(Guid windowsVariantId);
    WindowsVariant? GetWindowsVariant(Guid windowsVariantId);
    bool DeleteWindowsVariant(Guid windowsVariant);
}

public class ApplicationRepository(StorefrontDbContext dbContext): IApplicationRepository
{
    public IQueryable<Application> GetLatestApplications(int maxLength = 10)
    {
        return dbContext.Applications
        .OrderByDescending(a => a.CreatedAt)
        .Take(maxLength);
    }

    public IQueryable<Application> GetMostPopularApplications(int maxLength = 10)
    {
        return dbContext.Applications
        .Take(maxLength);
    }
    public Application? GetApplication(Guid id)
    {
        return dbContext.Applications.Find(id);
    }

    public Application CreateApplication(Application application)
    {
        dbContext.Applications.Add(application);
        dbContext.SaveChanges();
        return application;
    }

    public AndroidVariant CreateAndroidVariant(AndroidVariant androidVariant, Guid releaseId)
    {
        var release = dbContext.AndroidReleases.Find(releaseId) ?? throw new ArgumentException($"Release with ID {releaseId} not found.", nameof(releaseId));
        release?.Variants.Add(androidVariant);
        dbContext.SaveChanges();

        return androidVariant;
    }

    public MacOsVariant CreateMacOSVariant(MacOsVariant macOsVariant, Guid releaseId)
    {
        var release = dbContext.MacOsReleases.Find(releaseId) ?? throw new ArgumentException($"Release with ID {releaseId} not found.", nameof(releaseId));
        release?.Variants.Add(macOsVariant);
        dbContext.SaveChanges();

        return macOsVariant;
    }

    public WindowsVariant CreateWindowsVariant(WindowsVariant windowsVariant, Guid releaseId)
    {
        var release = dbContext.WindowsReleases.Find(releaseId) ?? throw new ArgumentException($"Release with ID {releaseId} not found.", nameof(releaseId));
        release?.Variants.Add(windowsVariant);
        dbContext.SaveChanges();

        return windowsVariant;
    }

    public AndroidRelease? GetAndroidRelease(Guid id)
    {
        return dbContext.AndroidReleases.Find(id);
    }
    public MacOsRelease? GetMacOSRelease(Guid id)
    {
        return dbContext.MacOsReleases.Find(id);
    }

    public WindowsRelease? GetWindowsRelease(Guid id)
    {
        return dbContext.WindowsReleases.Find(id);
    }

    public MacOsVariant? GetMacOSVariant(Guid macOsVariantId)
    {
       return dbContext.MacOsVariants.Find(macOsVariantId);
    }

    public AndroidVariant? GetAndroidVariant(Guid androidVariantId)
    {
        return dbContext.AndroidVariants.Find(androidVariantId);
    }

    public WindowsVariant? GetWindowsVariant(Guid windowsVariantId)
    {
        return dbContext.WindowsVariants.Find(windowsVariantId);
    }

    public bool DeleteWindowsVariant(Guid windowsVariantId)
    {
        var variant = dbContext.WindowsVariants.Find(windowsVariantId);
        if (variant == null)
        {
            return false;
        }
        dbContext.WindowsVariants.Remove(variant);
        return true;
    }
}