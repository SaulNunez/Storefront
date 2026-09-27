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
    Task UploadApplicationIcon(Guid applicationId, Stream stream);
}

public class ApplicationService(IApplicationRepository applicationRepository, IApplicationObjectStorageRepository applicationObjectStorage) : IApplicationService
{
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
        var application = new Models.Application
        {
            Name = applicationInput.ApplicationName,
            Description = applicationInput.ApplicationDescription,
            ElevatorPitch = applicationInput.ShortDescription,
            PhotoUrls = [],
            OwnerId = userId
        };

        return ApplicationDao.FromEntity(applicationRepository.CreateApplication(application));
    }

    public async Task<List<ApplicationDao>> GetDeveloperApplications(string userId, int take = 10, int skip = 0)
    {
        return await applicationRepository.GetApplicationsByDeveloper(userId, take, skip).Select(ApplicationDao.MapFromEntity).ToListAsync();
    }

    public async Task UploadApplicationIcon(Guid applicationId, Stream stream)
    {
        var application = applicationRepository.GetApplication(applicationId) ?? throw new NotFoundException($"Application with ID {applicationId} not found!");
        var objectKey = $"{application.Id}/listing/icons/store_icon_512.png";
        await applicationObjectStorage.UploadAppIcon("application", objectKey, stream);
    }
}