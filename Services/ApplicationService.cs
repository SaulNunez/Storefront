using Storefront.Models;
using Storefront.Models.DAO;
using Storefront.Models.Exceptions;
using Storefront.Services.Converters;
using Storefront.Models.Enums;
using Storefront.Models.Inputs;
using Storefront.Repositories;
using Storefront.Models.DAO.Windows;

namespace Storefront.Services;

public interface IApplicationService
{
    ApplicationDao CreateApplication(ApplicationInput applicationInput);
    ApplicationDao GetApplication(Guid id);
    List<ApplicationDao> GetLatestApplications(int maxLength = 10);
    List<ApplicationDao> GetMostPopularApplications(int maxLength = 10);
    HomeScreenDao GetHomeScreenData();
    Task<string> UploadAndroid(Guid applicationId, Guid releaseId, AndroidCpuPlatform targetPlatform, string clientFileName);
    Task<string> CreateAndroidVariantUploadLink(Guid windowsVariantId);
    Task<Guid> CreateMacOSVariant(Guid applicationId, Guid releaseId, MacOSPlatforms targetPlatform, string clientFileName);
    Task<string> CreateMacOsVariantUploadLink(Guid macOsVariantId);
    Task<Guid> UploadWindowsAsync(Guid applicationId, Guid releaseId, WindowsCpuPlatform targetPlatform, string clientFileName);
    Task<string> CreateWindowsVariantUploadLink(Guid windowsVariantId);
    WindowsVariantDao? GetWindowsVariant(Guid variantId);
    void DeleteWindowsVariant(Guid variantId);
    Task<List<ApplicationDao>> GetDeveloperApplications(string userId, int take = 10, int skip = 0);
}

public class ApplicationService(IApplicationRepository applicationRepository, IApplicationObjectStorageRepository applicationObjectStorage) : IApplicationService
{
    public ApplicationDao GetApplication(Guid id)
    {
        var application = applicationRepository.GetApplication(id) ?? throw new NotFoundException($"Application with ID {id} not found!");
        return application.ToDao();
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
        var latestApplications = applicationRepository.GetLatestApplications(maxLength).ToList();

        return [.. latestApplications.Select(a => a.ToDao())];
    }

    public List<ApplicationDao> GetMostPopularApplications(int maxLength = 10)
    {
        var latestApplications = applicationRepository.GetMostPopularApplications(maxLength).ToList();

        return [.. latestApplications.Select(a => a.ToDao())];
    }

    public ApplicationDao CreateApplication(ApplicationInput applicationInput)
    {
        var application = new Application
        {
            Name = applicationInput.ApplicationName,
            Description = applicationInput.ApplicationDescription,
            ElevatorPitch = applicationInput.ShortDescription,
            PhotoUrls = [],
            CreatedAt = DateTimeOffset.UtcNow
        };

        return applicationRepository.CreateApplication(application).ToDao();
    }

    public async Task<string> UploadAndroid(Guid applicationId, Guid releaseId, AndroidCpuPlatform targetPlatform, string clientFileName)
    {
        var application = applicationRepository.GetApplication(applicationId) ?? throw new NotFoundException($"Application with ID {applicationId} not found!");
        var release = applicationRepository.GetMacOSRelease(releaseId) ?? throw new NotFoundException($"Release with ID {releaseId} not found!");

        var platform = targetPlatform switch
        {
            AndroidCpuPlatform.amr_64_v8a => $"arm64",
            AndroidCpuPlatform.armeabi_v7a => $"arm",
            AndroidCpuPlatform.x86 => $"x86",
            AndroidCpuPlatform.x86_64 => $"x86_64",
            _ => throw new ArgumentOutOfRangeException(nameof(targetPlatform), "Unsupported Windows platform")
        };
        var sanitizedName = application.Name.ToLower().Replace(" ", "_").Replace(".", "_");
        var sanitizedVersionId = release.VersionId.Replace(".", "_");
        var extension = Path.GetExtension(clientFileName);
        if(extension == null || extension == "")
        {
            throw new ArgumentException("The provided file name does not have a valid extension.", nameof(clientFileName));
        }
        if(extension != ".apk")
        {
            throw new ArgumentException("The provided file name does not have a supported Android extension.", nameof(clientFileName));
        }
        var fileName = $"{sanitizedName}_android_{platform}_{sanitizedVersionId}.{extension}";
        var objectKey = $"{applicationId}/android_releases/{releaseId}/{fileName}";
        var createPath = await applicationObjectStorage.CreateApplicationUploadLink("applications", objectKey);
        var variant = new AndroidVariant
        {
            ContentLocation = objectKey,
            CpuPlatform = targetPlatform
        };
        applicationRepository.CreateAndroidVariant(variant, releaseId);

        return createPath;
    }

    public async Task<string> CreateAndroidVariantUploadLink(Guid windowsVariantId)
    {
        var androidVariant = applicationRepository.GetAndroidVariant(windowsVariantId) ?? throw new NotFoundException($"Android Variant with ID {windowsVariantId} not found!");
        var createPath = await applicationObjectStorage.CreateApplicationUploadLink("applications", androidVariant.ContentLocation);

        return createPath;
    }

    public async Task<Guid> CreateMacOSVariant(Guid applicationId, Guid releaseId, MacOSPlatforms targetPlatform, string clientFileName)
    {
        var application = applicationRepository.GetApplication(applicationId) ?? throw new NotFoundException($"Application with ID {applicationId} not found!");
        var release = applicationRepository.GetMacOSRelease(releaseId) ?? throw new NotFoundException($"Release with ID {releaseId} not found!");

        var platform = targetPlatform switch
        {
            MacOSPlatforms.INTEL => $"intel",
            MacOSPlatforms.APPLE_SILICON => $"apple_silicon",
            MacOSPlatforms.UNIVESAL_INTEL_APPLE_SILICON => $"universal",
            MacOSPlatforms.POWERPC => $"powerpc",
            MacOSPlatforms.UNIVERSAL_PPC_INTEL => $"universal_ppc_intel",
            MacOSPlatforms.MOTOROLA_68K => $"motorola_68k",
            _ => throw new ArgumentOutOfRangeException(nameof(targetPlatform), "Unsupported MacOS platform")
        };
        var sanitizedName = application.Name.ToLower().Replace(" ", "_").Replace(".", "_");
        var sanitizedVersionId = release.VersionId.Replace(".", "_");
        var extension = Path.GetExtension(clientFileName);
        if(extension == null || extension == "")
        {
            throw new ArgumentException("The provided file name does not have a valid extension.", nameof(clientFileName));
        }
        if(extension != ".dmg" && extension != ".pkg" && extension != ".zip" && extension != ".tar.gz")
        {
            throw new ArgumentException("The provided file name does not have a supported MacOS extension.", nameof(clientFileName));
        }
        var fileName = $"{sanitizedName}_macos_{platform}_{sanitizedVersionId}.{extension}";

        var objectKey = $"{applicationId}/android_releases/{releaseId}/{fileName}";
        
        var variant = new MacOsVariant
        {
            ContentLocation = objectKey,
            CpuPlatform = targetPlatform
        };
        return applicationRepository.CreateMacOSVariant(variant, releaseId).Id;
    }

    public async Task<string> CreateMacOsVariantUploadLink(Guid macOsVariantId)
    {
        var macOsVariant = applicationRepository.GetMacOSVariant(macOsVariantId) ?? throw new NotFoundException($"MacOS Variant with ID {macOsVariantId} not found!");
        var createPath = await applicationObjectStorage.CreateApplicationUploadLink("applications", macOsVariant.ContentLocation);

        return createPath;
    }

    public async Task<Guid> UploadWindowsAsync(Guid applicationId, Guid releaseId, WindowsCpuPlatform targetPlatform, string clientFileName)
    {
        var application = applicationRepository.GetApplication(applicationId) ?? throw new NotFoundException($"Application with ID {applicationId} not found!");
        var release = applicationRepository.GetMacOSRelease(releaseId) ?? throw new NotFoundException($"Release with ID {releaseId} not found!");

        var platform = targetPlatform switch
        {
            WindowsCpuPlatform.X86 => $"x86",
            WindowsCpuPlatform.X86_64 => $"x86_64",
            WindowsCpuPlatform.ARM64 => $"arm64",
            WindowsCpuPlatform.ARM => $"arm",
            _ => throw new ArgumentOutOfRangeException(nameof(targetPlatform), "Unsupported Windows platform")
        };
        var sanitizedName = application.Name.ToLower().Replace(" ", "_").Replace(".", "_");
        var sanitizedVersionId = release.VersionId.Replace(".", "_");
        var extension = Path.GetExtension(clientFileName);
        if(extension == null || extension == "")
        {
            throw new ArgumentException("The provided file name does not have a valid extension.", nameof(clientFileName));
        }
        if(extension != ".exe" && extension != ".msi" && extension != ".msix" && extension != ".appx" && extension != ".zip" && extension != ".com" && extension != ".bat")
        {
            throw new ArgumentException("The provided file name does not have a supported MacOS extension.", nameof(clientFileName));
        }
        var fileName = $"{sanitizedName}_windows_{platform}_{sanitizedVersionId}.{extension}";
        var objectKey = $"{applicationId}/windows_releases/{releaseId}/{fileName}";
        var createPath = await applicationObjectStorage.CreateApplicationUploadLink("applications", objectKey);
        var variant = new WindowsVariant
        {
            ContentLocation = createPath,
            CpuPlatform = targetPlatform
        };
        
        return applicationRepository.CreateWindowsVariant(variant, releaseId).Id;
    }

    public async Task<string> CreateWindowsVariantUploadLink(Guid windowsVariantId)
    {
        var windowsVariant = applicationRepository.GetWindowsVariant(windowsVariantId) ?? throw new NotFoundException($"Windows Variant with ID {windowsVariantId} not found!");
        var createPath = await applicationObjectStorage.CreateApplicationUploadLink("applications", windowsVariant.ContentLocation);

        return createPath;
    }

    public WindowsVariantDao? GetWindowsVariant(Guid windowsVariantId)
    {
        var windowsVariant = applicationRepository.GetWindowsVariant(windowsVariantId) ?? throw new NotFoundException($"Windows Variant with ID {windowsVariantId} not found!");
        return new WindowsVariantDao
        (
            Id: windowsVariant.Id,
            ContentLocation: windowsVariant.ContentLocation,
            CpuPlatform: windowsVariant.CpuPlatform
        );
    }

    public void DeleteWindowsVariant(Guid variantId)
    {
        if(!applicationRepository.DeleteWindowsVariant(variantId))
        {
            throw new NotFoundException($"Windows Variant with ID {variantId} not found!");
        }
    }

    public Task<List<ApplicationDao>> GetDeveloperApplications(string userId, int take = 10, int skip = 0)
    {
        var applications = applicationRepository.GetApplicationsByDeveloper(userId, take, skip).OrderByDescending(a => a.CreatedAt).ToList();
        var applicationDaos = applications.Select(a => a.ToDao()).ToList();
        return Task.FromResult(applicationDaos);
    }
}