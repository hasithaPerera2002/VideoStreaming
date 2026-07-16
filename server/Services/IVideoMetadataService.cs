using server.Models;

namespace server.Services;

public interface IVideoMetadataService
{
    Task SaveAsync(VideoMetadata metadata);
    Task<VideoMetadata?> GetByIdAsync(string id);
    Task<List<VideoMetadata>> GetAllAsync();
    Task DeleteAsync(string id);
}