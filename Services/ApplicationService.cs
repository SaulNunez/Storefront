using Microsoft.EntityFrameworkCore;
using Storefront.Models;
using Storefront.Models.DAO;
using Storefront.Models.Exceptions;
using Storefront.Models.Enums;
using Storefront.Models.Inputs;
using Storefront.Repositories;

namespace Storefront.Services;

public interface IApplicationService
{
    ApplicationDao CreateApplication(ApplicationInput applicationInput, string userId);
    ApplicationDao GetApplication(Guid id);
    List<ApplicationDao> GetLatestApplications(int maxLength = 10);
    List<ApplicationDao> GetMostPopularApplications(int maxLength = 10);
    HomeScreenDao GetHomeScreenData();
    Task<List<ApplicationDao>> GetDeveloperApplications(string userId, int take = 10, int skip = 0);
    ApplicationDao GetDeveloperApplication(Guid applicationId, string userId);
    Task<ApplicationDao> UploadApplicationIcon(Guid applicationId, string userId, Stream stream, string contentType);
}

public class ApplicationService(
    IApplicationRepository applicationRepository,
    IAppCategoryRepository appCategoryRepository,
    IApplicationObjectStorageRepository applicationObjectStorage,
    IConfiguration configuration) : IApplicationService
{
    private const string ApplicationBucket = "application";

    public ApplicationDao GetApplication(Guid id)
    {
        var application = applicationRepository.GetApplication(id) ?? throw new NotFoundException($"Application with ID {id} not found!");
        return ApplicationDao.FromEntity(application);
    }

    public HomeScreenDao GetHomeScreenData()
    {
        var mostPopularApplications = GetMostPopularApplications(10);
        var latestApplications = GetLatestApplications(10);

        return new HomeScreenDao
        {
            MostPopularApplications = mostPopularApplications,
            LatestApplications = latestApplications
        };
    }

    public List<ApplicationDao> GetLatestApplications(int maxLength = 10)
    {
        return applicationRepository.GetLatestApplications(maxLength).Select(ApplicationDao.MapFromEntity).ToList();
    }

    public List<ApplicationDao> GetMostPopularApplications(int maxLength = 10)
    {
        return applicationRepository.GetMostPopularApplications(maxLength).Select(ApplicationDao.MapFromEntity).ToList();
    }

    public ApplicationDao CreateApplication(ApplicationInput applicationInput, string userId)
    {
        if (!appCategoryRepository.CategoryExists(applicationInput.CategoryId))
        {
            throw new ArgumentException($"Category {applicationInput.CategoryId} does not exist.", nameof(applicationInput));
        }

        var application = new Models.Application
        {
            Name = applicationInput.ApplicationName,
            Description = applicationInput.ApplicationDescription,
            ElevatorPitch = applicationInput.ShortDescription,
            PhotoUrls = [],
            OwnerId = userId,
            CategoryId = applicationInput.CategoryId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        return ApplicationDao.FromEntity(applicationRepository.CreateApplication(application));
    }

    public async Task<List<ApplicationDao>> GetDeveloperApplications(string userId, int take = 10, int skip = 0)
    {
        return await applicationRepository.GetApplicationsByDeveloper(userId, take, skip).Select(ApplicationDao.MapFromEntity).ToListAsync();
    }

    public ApplicationDao GetDeveloperApplication(Guid applicationId, string userId)
    {
        return ApplicationDao.FromEntity(GetOwnedApplication(applicationId, userId));
    }

    public async Task<ApplicationDao> UploadApplicationIcon(Guid applicationId, string userId, Stream stream, string contentType)
    {
        var application = GetOwnedApplication(applicationId, userId);
        var publicUrl = GetPublicStorageUrl();
        var objectKey = $"{application.Id}/listing/icons/store_icon_512.png";
        await applicationObjectStorage.UploadAppIcon(ApplicationBucket, objectKey, stream, contentType);

        // The object key never changes, so a version query string makes browsers and CDNs pick up a replaced icon.
        application.StoreIconUrl = $"{publicUrl}/{objectKey}?v={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        return ApplicationDao.FromEntity(applicationRepository.UpdateApplication(application));
    }

    /// <summary>
    /// Loads an application for its owner. Someone else's application is reported as missing so its id is not leaked.
    /// </summary>
    private Models.Application GetOwnedApplication(Guid applicationId, string userId)
    {
        var application = applicationRepository.GetApplication(applicationId);
        if (application is null || application.OwnerId != userId)
        {
            throw new NotFoundException($"Application with ID {applicationId} not found!");
        }
        return application;
    }

    private string GetPublicStorageUrl()
    {
        var publicUrl = configuration["S3:PublicUrl"];
        if (string.IsNullOrWhiteSpace(publicUrl))
        {
            throw new InvalidOperationException("S3:PublicUrl is not configured, so uploaded icons cannot be linked.");
        }
        return publicUrl.TrimEnd('/');
    }
}
