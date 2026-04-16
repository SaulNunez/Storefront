
using System.Web;
using Storefront.Models;
using Storefront.Models.DAO;
using Storefront.Models.Enums;
using Storefront.Models.Exceptions;
using Storefront.Models.Inputs;
using Storefront.Repositories;

namespace Storefront.Services.Application;
public class WindowsApplicationService(IApplicationRepository applicationRepository, WindowsRepository winApplicationRepository, IApplicationObjectStorageRepository applicationObjectStorage): BaseApplicationService<WindowsReleaseDao, WindowsReleaseInput, WindowsVariantInput>
{
    public override Task<string> CreateVariantDownloadLink(Guid variantId)
    {
        throw new NotImplementedException();
    }

    public override async Task<string> CreateVariantUploadLink(Guid variantId)
    {
        var windowsVariant = winApplicationRepository.GetVariant(variantId) ?? throw new NotFoundException($"Windows Variant with ID {variantId} not found!");
        var createPath = await applicationObjectStorage.CreateApplicationUploadLink("applications", windowsVariant.ObjectKeyInStorage);

        return createPath;
    }

    public override async Task<Guid> CreateVariant(Guid applicationId, Guid releaseId, WindowsVariantInput variantInput)
    {
        var application = applicationRepository.GetApplication(applicationId) ?? throw new NotFoundException($"Application with ID {applicationId} not found!");
        var release = winApplicationRepository.GetRelease(releaseId) ?? throw new NotFoundException($"Release with ID {releaseId} not found!");

        var platform = variantInput.TargetPlatform switch
        {
            WindowsCpuPlatform.X86 => $"x86",
            WindowsCpuPlatform.X86_64 => $"x86_64",
            WindowsCpuPlatform.ARM64 => $"arm64",
            WindowsCpuPlatform.ARM => $"arm",
            _ => throw new ArgumentOutOfRangeException(nameof(variantInput.TargetPlatform), "Unsupported Windows platform")
        };
        var sanitizedName = SanitizeAppName(application.Name);
        var sanitizedVersionId = SanitizeVersionId(release.VersionId);
        var extension = Path.GetExtension(variantInput.ClientFileName);
        if(extension == null || extension == "")
        {
            throw new ArgumentException("The provided file name does not have a valid extension.", nameof(variantInput.ClientFileName));
        }
        if(extension != ".exe" && extension != ".msi" && extension != ".msix" && extension != ".appx" && extension != ".zip" && extension != ".com" && extension != ".bat")
        {
            throw new ArgumentException("The provided file name does not have a supported MacOS extension.", nameof(variantInput.ClientFileName));
        }
        var fileName = $"{sanitizedName}_windows_{platform}_{sanitizedVersionId}.{extension}";
        var objectKey = $"{applicationId}/windows_releases/{releaseId}/{fileName}";
        var createPath = await applicationObjectStorage.CreateApplicationUploadLink("applications", objectKey);
        var variant = new WindowsVariant
        {
            ObjectKeyInStorage = createPath,
            CpuPlatform = variantInput.TargetPlatform
        };
        
        return winApplicationRepository.CreateVariant(variant, releaseId).Id;
    }

    public override Task<WindowsReleaseDao> GetRelease(Guid releaseId)
    {
        var release = winApplicationRepository.GetRelease(releaseId) ?? throw new NotFoundException($"Release with ID {releaseId} not found!");
        return Task.FromResult(WindowsReleaseDao.FromEntity(release));
    }

    public override IEnumerable<WindowsReleaseDao> GetApplicationReleases(Guid applicationRelease, int skip = 0, int take = 10)
    {
        var releases = winApplicationRepository.GetReleases(applicationRelease, skip, take);
        return releases.Select(WindowsReleaseDao.FromEntity);
    }

    public override async Task<WindowsReleaseDao> CreateRelease(Guid applicationId, WindowsReleaseInput releaseCreateInput)
    {
        var cleanedReleaseNotes = SanitizeReleaseNotes(releaseCreateInput.ReleaseNotes);
        var releaseEntity = new WindowsRelease
        {
          VersionId = releaseCreateInput.VersionId,
          ReleaseNotes = cleanedReleaseNotes
        };

        var release = await winApplicationRepository.CreateRelease(applicationId, releaseEntity);
        return WindowsReleaseDao.FromEntity(release);
    }
}