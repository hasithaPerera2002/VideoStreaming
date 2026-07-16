using Amazon.S3;
using Amazon.S3.Model;

namespace server.Services;

public class S3Service : IVideoStorageService
{
    private readonly IAmazonS3 _s3Client;
    private const string BucketName = "videos";

    public S3Service(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<string> UploadVideoAsync(Stream fileStream, string fileName)
    {
        var key = $"{Guid.NewGuid()}-{fileName}";

        var request = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = "video/mp4"
        };

        await _s3Client.PutObjectAsync(request);

        return key;
    }
    public async Task<string> GetVideoUrlAsync(string s3Key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = BucketName,
            Key = s3Key,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }
}