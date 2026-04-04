using System.Web;
using Storefront.Models;
using Storefront.Models.DAO;
using Storefront.Models.Inputs;
using Storefront.Repositories;

namespace Storefront.Services;

public interface IReleaseService
{
    Task<WindowsReleaseDao> GetWindowsRelease(Guid releaseId);
    IEnumerable<WindowsReleaseDao> GetAllApplicationsWindowsRelease(Guid applicationRelease, int skip= 0, int take = 10);
    Task<WindowsReleaseDao> CreateWindowsRelease(Guid applicationId, WindowsReleaseInput windowsReleaseInput);
}

public class RelaseService(IApplicationRepository applicationRepository, IApplicationObjectStorageRepository applicationObjectStorage) : IReleaseService
{
    public async Task<WindowsReleaseDao> CreateWindowsRelease(Guid applicationId, WindowsReleaseInput windowsReleaseInput)
    {
        var cleanedReleaseNotes = HttpUtility.HtmlEncode(windowsReleaseInput.ReleaseNotes);
        var releaseEntity = new WindowsRelease
        {
          VersionId = windowsReleaseInput.VersionId,
          ReleaseNotes = cleanedReleaseNotes
        };

        var release = await applicationRepository.CreateWindowsRelease(applicationId, releaseEntity);
        return WindowsReleaseDao.FromEntity(release);
    }

    public IEnumerable<WindowsReleaseDao> GetAllApplicationsWindowsRelease(Guid applicationId, int skip= 0, int take = 10)
    {
        return applicationRepository.GetWindowsReleases(applicationId, skip, take).Select(WindowsReleaseDao.FromEntity);
    }

    public Task<WindowsReleaseDao> GetWindowsRelease(Guid releaseId)
    {
        return Task.FromResult(WindowsReleaseDao.FromEntity(applicationRepository.GetWindowsRelease(releaseId)));
    
    }
}