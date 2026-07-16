namespace server.Services;

public class S3Service : IVideoStorageService
{
    public Task<string> UploadVideoAsync(Stream fileStream, string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetVideoUrlAsync(string s3Key)
    {
        throw new NotImplementedException();
    }
}