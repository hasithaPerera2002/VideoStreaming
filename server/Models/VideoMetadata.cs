namespace server.Models;

public record VideoMetadata{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Uploader { get; init; }
    public DateTime UploadedAt { get; init; } = DateTime.UtcNow;
    public required string S3Key { get; init; }
    public string? ThumbnailUrl { get; init; }
    public long ViewCount { get; init; } = 0;
};