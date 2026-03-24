using Microsoft.AspNetCore.Mvc;
using Trackly.Api.DTOs;

namespace Trackly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TrackingController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TrackedPackageDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TrackedPackageDetailsDto> Create(CreateTrackingRequestDto request)
    {
        return StatusCode(StatusCodes.Status501NotImplemented, new
        {
            message = "Phase 3 will implement create tracking."
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TrackedPackageSummaryDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<TrackedPackageSummaryDto>> GetAll()
    {
        return Ok(Array.Empty<TrackedPackageSummaryDto>());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TrackedPackageDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TrackedPackageDetailsDto> GetById(Guid id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented, new
        {
            message = $"Phase 3 will implement lookup for package {id}."
        });
    }

    [HttpPost("{id:guid}/refresh")]
    [ProducesResponseType(typeof(TrackedPackageDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TrackedPackageDetailsDto> Refresh(Guid id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented, new
        {
            message = $"Phase 5 will implement refresh for package {id}."
        });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented, new
        {
            message = $"Phase 3 will implement delete for package {id}."
        });
    }
}
