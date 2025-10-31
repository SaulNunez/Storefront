using Storefront.Models;
using Storefront.Models.DAO;
using Storefront.Models.Exceptions;
using Storefront.Services.Converters;
using Storefront.Models.Enums;
using Storefront.Models.Inputs;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.AmazonS3;
using System.Threading.Tasks;

namespace Storefront.Services;

public interface IApplicationService
{
    ApplicationDao CreateApplication(ApplicationInput applicationInput);
    ApplicationDao GetApplication(Guid id);
    List<ApplicationDao> GetLatestApplications(int maxLength = 10);
    List<ApplicationDao> GetMostPopularApplications(int maxLength = 10);
}

public class ApplicationService(StorefrontDbContext dbContext, AmazonS3Client client) : IApplicationService
{
    public ApplicationDao GetApplication(Guid id)
    {
        var application = dbContext.Applications.Find(id) ?? throw new NotFoundException($"Application with ID {id} not found!");
        return application.ToDao();
    }

    public List<ApplicationDao> GetLatestApplications(int maxLength = 10)
    {
        var latestApplications = dbContext.Applications
        .OrderByDescending(a => a.CreatedAt)
        .Take(maxLength)
        .ToList();

        return [.. latestApplications.Select(a => a.ToDao())];
    }

    public List<ApplicationDao> GetMostPopularApplications(int maxLength = 10)
    {
        //Change to use download count
        var latestApplications = dbContext.Applications
        .Take(maxLength)
        .ToList();

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
        dbContext.Applications.Add(application);
        dbContext.SaveChanges();
        return application.ToDao();
    }
    public async Task<string> UploadAndroid(Guid applicationId, Guid releaseId, string fileName, string boundary, Stream contentStream, CancellationToken cancellationToken)
    {
        if(fileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
        {
            throw new ArgumentException("The provided file name contains invalid characters.", nameof(fileName));
        }

        var release = dbContext.AndroidReleases.Find(releaseId) ?? throw new NotFoundException($"Release with ID {releaseId} not found!");
        var objectKey = $"{applicationId}/android_releases/{releaseId}/{fileName}";
        var createPath = await CreateGenericPath("applications", objectKey);
        var variant = new AndroidVariant
        {
            ContentLocation = objectKey,
            CpuPlatform = AndroidCpuPlatform.All
        };
        release.Variants.Add(variant);
        await dbContext.SaveChangesAsync(cancellationToken);

        return createPath;
    }

    public async Task UploadMacOs(Guid applicationId, Guid releaseId, string fileName, string boundary, Stream contentStream, MacOSPlatforms targetPlatform, CancellationToken cancellationToken)
    {
        if(fileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
        {
            throw new ArgumentException("The provided file name contains invalid characters.", nameof(fileName));
        }

        var release = dbContext.MacOsReleases.Find(releaseId) ?? throw new NotFoundException($"Release with ID {releaseId} not found!");
        var objectKey = $"{applicationId}/android_releases/{releaseId}/{fileName}";
        var createPath = await CreateGenericPath("applications", objectKey);
        var variant = new MacOsVariant
        {
            ContentLocation = createPath,
            CpuPlatform = targetPlatform
        };
        release.Variants.Add(variant);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UploadWindowsAsync(Guid applicationId, Guid releaseId, string fileName, string boundary, Stream contentStream, WindowsCpuPlatform targetPlatform, CancellationToken cancellationToken)
    {
        if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
        {
            throw new ArgumentException("The provided file name contains invalid characters.", nameof(fileName));
        }

        var release = dbContext.WindowsReleases.Find(releaseId) ?? throw new NotFoundException($"Release with ID {releaseId} not found!");
        var objectKey = $"{applicationId}/android_releases/{releaseId}/{fileName}";
        var createPath = await CreateGenericPath("applications", objectKey);
        var variant = new WindowsApplicationVariant
        {
            ContentLocation = createPath,
            CpuPlatform = targetPlatform
        };
        release.Variants.Add(variant);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<string> CreateAndroidUploadPath(Guid applicationId, Guid releaseId, string fileName)
    {
        var objectKey = $"{applicationId}/android_releases/{releaseId}/{fileName}";
        var createPath = await CreateGenericPath("applications", objectKey);
        return createPath;
    }

    private async Task<string> CreateGenericPath(string bucket, string key)
    {
        //Create a multipart upload
        CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(bucket, key);
        UploadPartRequest req = new(bucket, key, createResp.UploadId, 1, null);
        string url = client.SignRequest(req, TimeSpan.FromSeconds(100));
        return url;
    }
}