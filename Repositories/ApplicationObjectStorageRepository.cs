using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.AmazonS3;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Storefront.Repositories;

public interface IApplicationObjectStorageRepository
{
    Task<string> CreateApplicationUploadLink(string bucket, string key);
    Task UploadAppIcon(string bucket, string key, Stream file, string contentType);
    Task UploadApplicationScreenshotsAsync(string bucket, string key, Stream file);
}

public class ApplicationObjectStorageRepository(AmazonS3Client client) : IApplicationObjectStorageRepository
{
    public async Task<string> CreateApplicationUploadLink(string bucket, string key)
    {
        //Create a multipart upload
        CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(bucket, key);
        UploadPartRequest req = new(bucket, key, createResp.UploadId, 1, null);
        string url = client.SignRequest(req, TimeSpan.FromSeconds(100));
        return url;
    }

    public async Task UploadAppIcon(string bucket, string key, Stream file, string contentType)
    {
        IUpload upload = client.CreateUpload(bucket, key)
                        .WithAccessControl(ObjectCannedAcl.PublicRead)
                        .WithCacheControl(CacheControlType.NoCache)
                        .WithContentType(contentType)
                        .WithEncryption();

        // Icons are small (the controller caps them), so buffer and send a single PUT; multipart is for large binaries.
        using var buffer = new MemoryStream();
        await file.CopyToAsync(buffer);
        var uploadResponse = await upload.UploadDataAsync(buffer.ToArray());
        if (!uploadResponse.IsSuccess)
        {
            throw new InvalidOperationException($"Uploading {key} to bucket {bucket} failed with status {uploadResponse.StatusCode}.");
        }
    }

    public async Task UploadApplicationScreenshotsAsync(string bucket, string key, Stream file)
    {
        IUpload upload = client.CreateUpload(bucket, key)
                        .WithAccessControl(ObjectCannedAcl.PublicRead)
                        .WithCacheControl(CacheControlType.NoCache)
                        .WithEncryption();

        var uploadResponse = await upload.UploadMultipartAsync(file);
    }
}