using Microsoft.AspNetCore.Mvc;
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
}