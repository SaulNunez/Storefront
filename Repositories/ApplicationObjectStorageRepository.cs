using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.AmazonS3;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

public interface IApplicationObjectStorageRepository
{
    Task<string> CreateApplicationUploadLink(string bucket, string key);
    Task UploadAppIcon(string bucket, string key, Stream file);
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

    public async Task UploadAppIcon(string bucket, string key, Stream file)
    {
        IUpload upload = client.CreateUpload(bucket, key)
                        .WithAccessControl(ObjectCannedAcl.PublicRead)
                        .WithCacheControl(CacheControlType.NoCache)
                        .WithEncryption();

        var uploadResponse = await upload.UploadMultipartAsync(file);
        
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