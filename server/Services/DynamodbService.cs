using server.Models;

namespace server.Services;

public class DynamodbService :IVideoMetadataService
{
    public Task SaveAsync(VideoMetadata metadata)
    {
        throw new NotImplementedException();
    }

    public Task<VideoMetadata?> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<VideoMetadata>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }
}