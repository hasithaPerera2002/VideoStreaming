using Microsoft.AspNetCore.Mvc;
using server.Models;
using server.Services;

namespace server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class VideoController : ControllerBase
{
    private readonly IVideoStorageService _storageService;
    private readonly IVideoMetadataService _metadataService;

    public VideoController(
        IVideoStorageService storageService,
        IVideoMetadataService metadataService)
    {
        _storageService = storageService;
        _metadataService = metadataService;
    }
    
    
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] string title, [FromForm] string uploader)
    {
        using var stream = file.OpenReadStream();
        var s3Key = await _storageService.UploadVideoAsync(stream, file.FileName);

        var metadata = new VideoMetadata
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            Uploader = uploader,
            S3Key = s3Key
        };

        await _metadataService.SaveAsync(metadata);
        return Ok(metadata);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var videos = await _metadataService.GetAllAsync();
        return Ok(videos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var video = await _metadataService.GetByIdAsync(id);
        if (video is null) return NotFound();
        return Ok(video);
    }
    
    [HttpGet("{id}/stream-url")]
    public async Task<IActionResult> GetStreamUrl(string id)
    {
        var video = await _metadataService.GetByIdAsync(id);
        if (video is null) return NotFound();

        var url = await _storageService.GetVideoUrlAsync(video.S3Key);
        return Ok(new { url });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _metadataService.DeleteAsync(id);
        return NoContent();
    }
    
}