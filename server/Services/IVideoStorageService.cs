namespace server.Services;

public interface IVideoStorageService
{
    Task<string> UploadVideoAsync(Stream fileStream, string fileName);
    Task<string> GetVideoUrlAsync(string s3Key);
}