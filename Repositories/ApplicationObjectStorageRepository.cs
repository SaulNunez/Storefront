using Genbox.SimpleS3.AmazonS3;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

public interface IApplicationObjectStorageRepository
{
    Task<string> CreateApplicationUploadLink(string bucket, string key);
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
}