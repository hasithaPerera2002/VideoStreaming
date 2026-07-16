using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using server.Models;

namespace server.Services;

public class DynamodbService :IVideoMetadataService
{
    private readonly ITable _table;

    public DynamodbService(IAmazonDynamoDB client)
    {
        _table = new TableBuilder(client, "VideoMetadata")
            .AddHashKey("id", DynamoDBEntryType.String)
            .Build();
    }

    public async Task SaveAsync(VideoMetadata metadata)
    {
        var doc = new Document
        {
            ["id"] = metadata.Id,
            ["title"] = metadata.Title,
            ["uploader"] = metadata.Uploader,
            ["uploadedAt"] = metadata.UploadedAt.ToString("o"),
            ["s3Key"] = metadata.S3Key,
            ["thumbnailUrl"] = metadata.ThumbnailUrl ?? "",
            ["viewCount"] = metadata.ViewCount
        };
        await _table.PutItemAsync(doc);
    }

    public async Task<VideoMetadata?> GetByIdAsync(string id)
    {
        var doc = await _table.GetItemAsync(id);
        return doc is null ? null : MapToMetadata(doc);
    }

    public async Task<List<VideoMetadata>> GetAllAsync()
    {
        var search = _table.Scan(new ScanFilter());
        var results = new List<VideoMetadata>();

        do
        {
            var docs = await search.GetNextSetAsync();
            results.AddRange(docs.Select(MapToMetadata));
        } while (!search.IsDone);

        return results;
    }

    public async Task DeleteAsync(string id)
    {
        await _table.DeleteItemAsync(id);
    }
    
    private static VideoMetadata MapToMetadata(Document doc) => new()
    {
        Id = doc["id"],
        Title = doc["title"],
        Uploader = doc["uploader"],
        UploadedAt = DateTime.Parse(doc["uploadedAt"]),
        S3Key = doc["s3Key"],
        ThumbnailUrl = doc["thumbnailUrl"],
        ViewCount = (long)doc["viewCount"]
    };
}