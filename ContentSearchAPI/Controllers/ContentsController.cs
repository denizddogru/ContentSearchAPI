using ContentSearchAPI.Application.DTOs;
using ContentSearchAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContentSearchAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentsController : ControllerBase
{
    private readonly IContentService _contentService;
    private readonly ILogger<ContentsController> _logger;

    public ContentsController(IContentService contentService, ILogger<ContentsController> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }

    /// <summary>
    /// Search for content with filters and pagination
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<ContentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ContentDto>>> Search(
        [FromQuery] ContentSearchRequest request,
        CancellationToken cancellationToken)
    {
        if (request.PageSize > 50)
        {
            return BadRequest("Page size cannot exceed 50");
        }

        var result = await _contentService.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get content by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var content = await _contentService.GetByIdAsync(id, cancellationToken);
        if (content == null)
        {
            return NotFound();
        }

        return Ok(content);
    }
}
